using FluentAssertions;

using FluentValidation.Results;

using MBA.Core.Data;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.FaturamentoCommands;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Core.Messages.Integration;
using MBA.Core.SharedDto.Aluno;
using MBA.Core.SharedDto.Aluno.Enum;
using MBA.MessageBus;
using MBA.Pagamentos.Application.Commands.RealizarPagamento;
using MBA.Pagamentos.Application.Services;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.Interfaces;

using Moq;

namespace MBA.Pagamentos.Testes.Applications.Commands;

// Cobre os ramos do handler nao exercitados por RealizarPagamentoCommandHandlerTests:
// pagamento nao permitido, matricula nula/nao-pagavel, valor divergente, falha ao alterar
// status e reuso de pagamento existente. Mocks sao campos para poder sobrescreve-los por teste.
public class RealizarPagamentoCommandHandlerBranchesTests
{
	private readonly Mock<IFaturamentoRepository> _repo = new();
	private readonly Mock<IMessageBus> _bus = new();
	private readonly Mock<IMediatorHandler> _mediator = new();
	private readonly Mock<IAlunoService> _aluno = new();

	public RealizarPagamentoCommandHandlerBranchesTests()
	{
		var uow = new Mock<IUnitOfWork>();
		uow.Setup(u => u.Commit()).ReturnsAsync(true);
		_repo.Setup(r => r.UnitOfWork).Returns(uow.Object);

		_bus.Setup(b => b.RequestAsync<AlterarStatusMatriculaIntegrationEvent, ResponseMessage>(
				It.IsAny<AlterarStatusMatriculaIntegrationEvent>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ResponseMessage(new ValidationResult()));

		_aluno.Setup(s => s.ObterStatusMatriculaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Guid id, CancellationToken _) => new MatriculaStatusDto
			{
				Id = id,
				AlunoId = Guid.NewGuid(),
				CursoId = Guid.NewGuid(),
				Status = StatusMatricula.PendentePagamento.ToString(),
				PodeSerPaga = true
			});
	}

	private RealizarPagamentoCommandHandler Criar()
		=> new(_repo.Object, _bus.Object, _mediator.Object, _aluno.Object);

	private static RealizarPagamentoCommand ComandoValido(decimal valor = 2500m, bool podeRealizar = true)
		=> new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), podeRealizar, "Curso Teste",
			valor, DateTime.Now, null, "Ativa",
			"5493813493498874", "JAIRO A SOUZA", "12/26", "123");

	[Fact]
	public async Task Deve_rejeitar_quando_pagamento_nao_pode_ser_realizado()
	{
		var resultado = await Criar().Handle(ComandoValido(podeRealizar: false), CancellationToken.None);

		resultado.Should().BeFalse();
		_mediator.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
	}

	[Fact]
	public async Task Deve_rejeitar_quando_matricula_nao_encontrada()
	{
		_aluno.Setup(s => s.ObterStatusMatriculaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((MatriculaStatusDto?)null);

		var resultado = await Criar().Handle(ComandoValido(), CancellationToken.None);

		resultado.Should().BeFalse();
	}

	[Fact]
	public async Task Deve_rejeitar_quando_matricula_nao_pode_ser_paga()
	{
		_aluno.Setup(s => s.ObterStatusMatriculaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new MatriculaStatusDto
			{
				Id = Guid.NewGuid(),
				AlunoId = Guid.NewGuid(),
				CursoId = Guid.NewGuid(),
				Status = "Cancelada",
				PodeSerPaga = false
			});

		var resultado = await Criar().Handle(ComandoValido(), CancellationToken.None);

		resultado.Should().BeFalse();
	}

	[Fact]
	public async Task Deve_recusar_quando_valor_diverge_do_pagamento_existente()
	{
		var existente = new Pagamento(Guid.NewGuid(), 100m, DateTime.Now);
		_repo.Setup(r => r.ObterPorMatriculaIdAsync(It.IsAny<Guid>())).ReturnsAsync(existente);

		var resultado = await Criar().Handle(ComandoValido(valor: 999m), CancellationToken.None);

		resultado.Should().BeFalse();
		_bus.Verify(b => b.PublishAsync(It.IsAny<PagamentoRecusadoIntegrationEvent>(), It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Fact]
	public async Task Deve_rejeitar_quando_alterar_status_da_matricula_falha()
	{
		_repo.Setup(r => r.ObterPorMatriculaIdAsync(It.IsAny<Guid>())).ReturnsAsync((Pagamento?)null);
		_bus.Setup(b => b.RequestAsync<AlterarStatusMatriculaIntegrationEvent, ResponseMessage>(
				It.IsAny<AlterarStatusMatriculaIntegrationEvent>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ResponseMessage(
				new ValidationResult(new[] { new ValidationFailure("status", "falhou") })));

		var resultado = await Criar().Handle(ComandoValido(), CancellationToken.None);

		resultado.Should().BeFalse();
	}

	[Fact]
	public async Task Deve_reusar_pagamento_existente_sem_adicionar_novo()
	{
		var existente = new Pagamento(Guid.NewGuid(), 2500m, DateTime.Now);
		_repo.Setup(r => r.ObterPorMatriculaIdAsync(It.IsAny<Guid>())).ReturnsAsync(existente);

		var resultado = await Criar().Handle(ComandoValido(valor: 2500m), CancellationToken.None);

		resultado.Should().BeTrue();
		_repo.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>()), Times.Never);
	}
}
