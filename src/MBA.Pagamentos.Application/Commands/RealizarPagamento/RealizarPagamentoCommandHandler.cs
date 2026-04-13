using MBA.Core.DomainObjects;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.FaturamentoCommands;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Core.Messages.Integration;
using MBA.Core.SharedDto.Aluno.Enum;
using MBA.MessageBus;
using MBA.Pagamentos.Application.Services;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.Interfaces;
using MBA.Pagamentos.Domain.ValueObjects;

using MediatR;


namespace MBA.Pagamentos.Application.Commands.RealizarPagamento;

public class RealizarPagamentoCommandHandler(
	IFaturamentoRepository faturamentoRepository,
	IMessageBus bus,
	IMediatorHandler mediatorHandler,
	IAlunoService alunoService) : IRequestHandler<RealizarPagamentoCommand, bool>
{
	private Guid _raizAgregacao;

	public async Task<bool> Handle(
		RealizarPagamentoCommand request,
		CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;

		// 1️⃣ Validação do command
		if (!ValidarRequisicaoAsync(request))
			return false;

		// 2️⃣ Matrícula obrigatória
		if (request.MatriculaCursoId == Guid.Empty)
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(
					_raizAgregacao,
					nameof(Pagamento),
					"Matrícula inválida para realização de pagamento."));
			return false;
		}

		// 2.1️⃣ Cliente declarou que o pagamento NÃO pode ser realizado → rejeita cedo
		if (!request.PagamentoPodeSerRealizado)
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(
					_raizAgregacao,
					nameof(Pagamento),
					"O pagamento não pode ser realizado para esta matrícula."));
			return false;
		}

		// 2.2️⃣ Valida na Aluno API se a matrícula realmente está em PendentePagamento
		var statusMatricula = await alunoService.ObterStatusMatriculaAsync(request.MatriculaCursoId, cancellationToken);
		if (statusMatricula is null)
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(
					_raizAgregacao,
					nameof(Pagamento),
					"Matrícula não encontrada."));
			return false;
		}

		if (!statusMatricula.PodeSerPaga)
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(
					_raizAgregacao,
					nameof(Pagamento),
					$"Matrícula em status {statusMatricula.Status}; não pode receber pagamento."));
			return false;
		}

		// 3️⃣ Busca pagamento (PODE ser null)
		var resultado = await ObterPagamentoMatriculaCurso(request.MatriculaCursoId);

		if (!resultado.Sucesso)
			return false;

		var pagamento = resultado.Pagamento;

		// 4️⃣ BLOQUEIO SOMENTE se já estiver APROVADO
		if (pagamento is not null && pagamento.PossuiPagamentoAprovado())
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(
					_raizAgregacao,
					nameof(Pagamento),
					"Pagamento desta matrícula já se encontra aprovado."));
			return false;
		}

		// 5️⃣ Validação de valor
		var valorReferencia = pagamento?.Valor ?? request.Valor;

		if (!ValidarValorPagamentoMatriculaCurso(request.Valor, valorReferencia))
		{
			await PublicarPagamentoRecusadoAsync(request,
				"Valor de pagamento diverge do valor desta matricula", cancellationToken);
			return false;
		}

		// 6️⃣ Dados do cartão
		var dadosCartao = new DadosCartao(
			request.NumeroCartao,
			request.NomeTitularCartao,
			request.ValidadeCartao,
			request.CvvCartao);

		// 7️⃣ Criação ou reaproveitamento
		if (pagamento is null)
		{
			pagamento = new Pagamento(
				request.MatriculaCursoId,
				request.Valor,
				DateTime.Now.Date);

			await faturamentoRepository.AdicionarAsync(pagamento);
		}

		// 8️⃣ Confirma pagamento (DOMÍNIO decide)
		try
		{
			pagamento.ConfirmarPagamento(
				DateTime.Now,
				Guid.NewGuid().ToString(),
				dadosCartao);
		}
		catch (DomainException ex)
		{
			pagamento.RecusarPagamento();

			await mediatorHandler.PublicarEventoRaiz(
				new PagamentoRecusadoEvent(
					request.MatriculaCursoId,
					request.AlunoId,
					request.CursoId,
					ex.Message));

			await PublicarPagamentoRecusadoAsync(request, ex.Message, cancellationToken);
			return false;
		}

		// 9️⃣ Commit
		var clienteResult = await AlterarStatusMatricula(request.MatriculaCursoId, StatusMatricula.PagamentoRealizado);
		if (!clienteResult.ValidationResult.IsValid)
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(
					_raizAgregacao,
					nameof(Pagamento),
					"ERRO AO ALTERAR O STATUS DA MATRICULA."));

			await PublicarPagamentoRecusadoAsync(request,
				"Falha ao alterar status da matrícula após confirmação do pagamento",
				cancellationToken);
			return false;
		}

		await faturamentoRepository.UnitOfWork.Commit();

		// Evento de domínio (MediatR — in-process)
		await mediatorHandler.PublicarEventoRaiz(
			new PagamentoConfirmadoEvent(
				request.MatriculaCursoId,
				request.AlunoId,
				request.CursoId,
				true));

		// Evento de integração (broker — entre bounded contexts)
		await bus.PublishAsync(
			new PagamentoConfirmadoIntegrationEvent(
				request.MatriculaCursoId,
				request.AlunoId,
				request.CursoId),
			cancellationToken);

		return true;
	}

	private bool ValidarRequisicaoAsync(RealizarPagamentoCommand request)
	{
		request.DefinirValidacao(new RealizarPagamentoCommandValidator().Validate(request));
		if (!request.EhValido())
		{
			foreach (var erro in request.Erros)
				mediatorHandler
					.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento), erro))
					.GetAwaiter().GetResult();
			return false;
		}

		return true;
	}

	private async Task<(bool Sucesso, Pagamento? Pagamento)>
		ObterPagamentoMatriculaCurso(Guid matriculaId)
	{
		var pagamento =
			await faturamentoRepository.ObterPorMatriculaIdAsync(matriculaId);

		if (pagamento is not null && pagamento.PossuiPagamentoAprovado())
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(
					_raizAgregacao,
					nameof(Pagamento),
					"Pagamento desta matrícula já se encontra paga"
				)
			);

			return (false, pagamento);
		}

		return (true, pagamento);
	}

	private bool ValidarValorPagamentoMatriculaCurso(decimal valorInformado, decimal valorMatricula)
	{
		if (valorInformado != valorMatricula)
		{
			mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento),
				"Valor de pagamento diverge do valor desta matricula")).GetAwaiter().GetResult();
			return false;
		}

		return true;
	}

	private async Task<ResponseMessage> AlterarStatusMatricula(Guid matriculaId, StatusMatricula status)
	{
		var alterarMatricula = new AlterarStatusMatriculaIntegrationEvent(matriculaId, status);

		return await bus.RequestAsync<AlterarStatusMatriculaIntegrationEvent, ResponseMessage>(alterarMatricula);
	}

	private async Task PublicarPagamentoRecusadoAsync(
		RealizarPagamentoCommand request,
		string motivoRecusa,
		CancellationToken cancellationToken)
	{
		await bus.PublishAsync(new PagamentoRecusadoIntegrationEvent(
			request.MatriculaCursoId,
			request.AlunoId,
			request.CursoId,
			motivoRecusa),
			cancellationToken);
	}
}