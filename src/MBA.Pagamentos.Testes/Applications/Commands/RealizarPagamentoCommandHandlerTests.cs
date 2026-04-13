using FluentAssertions;

using MBA.Core.Data;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.FaturamentoCommands;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Core.SharedDto;
using MBA.Core.SharedDto.Aluno;
using MBA.Core.SharedDto.Aluno.Enum;
using MBA.MessageBus;
using MBA.Pagamentos.Application.Commands.RealizarPagamento;
using MBA.Pagamentos.Application.Services;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.Interfaces;
using MBA.Pagamentos.Domain.ValueObjects;

using Moq;

namespace MBA.Pagamentos.Testes.Applications.Commands;

public class RealizarPagamentoCommandHandlerTests
{
	private readonly Mock<IFaturamentoRepository> _faturamentoRepositoryMock;
	private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
	private readonly RealizarPagamentoCommandHandler _handler;

	public RealizarPagamentoCommandHandlerTests()
	{
		_faturamentoRepositoryMock = new Mock<IFaturamentoRepository>();
		var messageBusMock = new Mock<IMessageBus>();
		_mediatorHandlerMock = new Mock<IMediatorHandler>();

		var unitOfWorkMock = new Mock<IUnitOfWork>();
		unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
		_faturamentoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

		var alunoServiceMock = new Mock<IAlunoService>();
		alunoServiceMock
			.Setup(s => s.ObterStatusMatriculaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Guid matriculaId, CancellationToken _) => new MatriculaStatusDto
			{
				Id = matriculaId,
				AlunoId = Guid.NewGuid(),
				CursoId = Guid.NewGuid(),
				Status = StatusMatricula.PendentePagamento.ToString(),
				PodeSerPaga = true
			});

		_handler = new RealizarPagamentoCommandHandler(
			_faturamentoRepositoryMock.Object,
			messageBusMock.Object,
			_mediatorHandlerMock.Object,
			alunoServiceMock.Object
		);
	}

	private RealizarPagamentoCommand CriarComandoValido()
	{
		var matriculaId = Guid.NewGuid();
		var valor = 2500.00m;
		Guid.NewGuid();

		var matriculaCurso = new MatriculaCursoDto
		{
			Id = matriculaId,
			AlunoId = Guid.NewGuid(),
			CursoId = Guid.NewGuid(),
			Valor = valor,
			PagamentoPodeSerRealizado = true
		};

		return new RealizarPagamentoCommand(matriculaId,
			matriculaCurso.CursoId,
			matriculaCurso.AlunoId,
			matriculaCurso.PagamentoPodeSerRealizado,
			matriculaCurso.NomeCurso,
			matriculaCurso.Valor,
			matriculaCurso.DataMatricula,
			matriculaCurso.DataConclusao,
			matriculaCurso.EstadoMatricula,
			"5493813493498874",
			"JAIRO A SOUZA",
			"12/26",
			"123");
	}


	[Fact]
	public async Task Deve_retornar_false_quando_comando_invalido()
	{
		var comando = new RealizarPagamentoCommand(Guid.Empty, Guid.Empty, Guid.Empty, false, string.Empty, 0.00m,
			DateTime.Now,
			null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		var resultado = await _handler.Handle(comando, CancellationToken.None);

		resultado.Should().BeFalse();
		_mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()),
			Times.AtLeastOnce);
	}

	[Fact]
	public async Task Deve_retornar_false_quando_matricula_for_guid_empty()
	{
		var comando = CriarComandoComMatriculaInvalida();

		var resultado = await _handler.Handle(comando, CancellationToken.None);

		resultado.Should().BeFalse();

		_mediatorHandlerMock.Verify(
			m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()),
			Times.Once
		);
	}

	[Fact]
	public async Task Deve_confirmar_pagamento_com_sucesso()
	{
		var comando = CriarComandoValido();

		_faturamentoRepositoryMock.Setup(r => r.ObterPorMatriculaIdAsync(comando.MatriculaCursoId))
			.ReturnsAsync((Pagamento?)null);

		var resultado = await _handler.Handle(comando, CancellationToken.None);

		resultado.Should().BeTrue();
		_faturamentoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>()), Times.Once);
		_mediatorHandlerMock.Verify(m => m.PublicarEventoRaiz(It.IsAny<PagamentoConfirmadoEvent>()), Times.Once);
	}

	[Fact]
	public async Task Deve_retornar_false_quando_pagamento_ja_confirmado()
	{
		var comando = CriarComandoValido();
		var pagamento = new Pagamento(comando.MatriculaCursoId, comando.Valor, DateTime.Now);
		pagamento.ConfirmarPagamento(DateTime.Now, "ABCUIYKJHKJSAHDKAS",
			new DadosCartao(comando.NumeroCartao, comando.NomeTitularCartao, comando.ValidadeCartao,
				comando.CvvCartao));

		_faturamentoRepositoryMock.Setup(r => r.ObterPorMatriculaIdAsync(comando.MatriculaCursoId))
			.ReturnsAsync(pagamento);

		var resultado = await _handler.Handle(comando, CancellationToken.None);

		resultado.Should().BeFalse();
		_mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.Once);
	}

	private RealizarPagamentoCommand CriarComandoComMatriculaInvalida()
	{
		return new RealizarPagamentoCommand(
			Guid.Empty,
			Guid.NewGuid(),
			Guid.NewGuid(),
			true,
			"Curso Teste",
			2500m,
			DateTime.Now,
			null,
			"Ativa",
			"5493813493498874",
			"JAIRO A SOUZA",
			"12/26",
			"123"
		);
	}
}