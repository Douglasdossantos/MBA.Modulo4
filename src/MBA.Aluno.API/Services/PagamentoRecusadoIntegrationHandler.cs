using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages.AlunoCommands;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Core.SharedDto.Aluno.Enum;
using MBA.MessageBus;

namespace MBA.Aluno.API.Services;

public class PagamentoRecusadoIntegrationHandler(
	IServiceProvider serviceProvider,
	IMessageBus bus,
	ILogger<PagamentoRecusadoIntegrationHandler> logger) : BackgroundService
{
	private const string SubscriptionId = "aluno-pagamento-recusado";

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		SetSubscriber(stoppingToken);
		bus.AdvancedBus.Connected += (_, _) => SetSubscriber(stoppingToken);
		return Task.CompletedTask;
	}

	private void SetSubscriber(CancellationToken cancellationToken)
	{
		bus.SubscribeAsync<PagamentoRecusadoIntegrationEvent>(
			SubscriptionId,
			async message => await ProcessarPagamentoRecusadoAsync(message),
			cancellationToken);
	}

	private async Task ProcessarPagamentoRecusadoAsync(PagamentoRecusadoIntegrationEvent message)
	{
		try
		{
			logger.LogInformation(
				"[PagamentoRecusadoIntegrationHandler] Recebido evento. MatriculaId: {MatriculaId}, Motivo: {MotivoRecusa}",
				message.MatriculaCursoId, message.MotivoRecusa);

			using var scope = serviceProvider.CreateScope();
			var matriculaRepository = scope.ServiceProvider.GetRequiredService<IMatriculaRepository>();
			var mediatorHandler = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

			var matricula = await matriculaRepository.ObterPorIdAsync(message.MatriculaCursoId);

			if (matricula is null)
			{
				logger.LogWarning(
					"[PagamentoRecusadoIntegrationHandler] Matrícula {MatriculaId} não encontrada. Evento ignorado.",
					message.MatriculaCursoId);
				return;
			}

			if (matricula.Status == StatusMatricula.PagamentoRecusado)
			{
				logger.LogInformation(
					"[PagamentoRecusadoIntegrationHandler] Matrícula {MatriculaId} já em PagamentoRecusado. Ignorado (idempotência).",
					message.MatriculaCursoId);
				return;
			}

			var comando = new AlterarStatusMatriculaCommand(
				message.MatriculaCursoId,
				StatusMatricula.PagamentoRecusado);

			var sucesso = await mediatorHandler.EnviarComandoRaiz(comando);

			if (sucesso)
				logger.LogInformation(
					"[PagamentoRecusadoIntegrationHandler] Matrícula {MatriculaId} marcada como PagamentoRecusado.",
					message.MatriculaCursoId);
			else
				logger.LogWarning(
					"[PagamentoRecusadoIntegrationHandler] Falha ao marcar matrícula {MatriculaId}.",
					message.MatriculaCursoId);
		}
		catch (Exception ex)
		{
			logger.LogError(ex,
				"[PagamentoRecusadoIntegrationHandler] Erro ao processar evento para matrícula {MatriculaId}: {Message}",
				message.MatriculaCursoId, ex.Message);
			throw;
		}
	}
}
