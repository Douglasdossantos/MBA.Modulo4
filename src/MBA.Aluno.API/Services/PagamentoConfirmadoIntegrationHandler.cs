using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages.AlunoCommands;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Core.SharedDto.Aluno.Enum;
using MBA.MessageBus;

namespace MBA.Aluno.API.Services;

public class PagamentoConfirmadoIntegrationHandler(
	IServiceProvider serviceProvider,
	IMessageBus bus,
	ILogger<PagamentoConfirmadoIntegrationHandler> logger) : BackgroundService
{
	private const string SubscriptionId = "aluno-pagamento-confirmado";

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		SetSubscriber(stoppingToken);
		bus.AdvancedBus.Connected += (_, _) => SetSubscriber(stoppingToken);
		return Task.CompletedTask;
	}

	private void SetSubscriber(CancellationToken cancellationToken)
	{
		bus.SubscribeAsync<PagamentoConfirmadoIntegrationEvent>(
			SubscriptionId,
			async message => await ProcessarPagamentoConfirmadoAsync(message),
			cancellationToken);
	}

	private async Task ProcessarPagamentoConfirmadoAsync(PagamentoConfirmadoIntegrationEvent message)
	{
		try
		{
			logger.LogInformation(
				"[PagamentoConfirmadoIntegrationHandler] Recebido evento. MatriculaId: {MatriculaId}",
				message.MatriculaCursoId);

			using var scope = serviceProvider.CreateScope();
			var matriculaRepository = scope.ServiceProvider.GetRequiredService<IMatriculaRepository>();
			var mediatorHandler = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

			var matricula = await matriculaRepository.ObterPorIdAsync(message.MatriculaCursoId);

			if (matricula is null)
			{
				logger.LogWarning(
					"[PagamentoConfirmadoIntegrationHandler] Matrícula {MatriculaId} não encontrada. Evento ignorado.",
					message.MatriculaCursoId);
				return;
			}

			if (matricula.Status == StatusMatricula.PagamentoRealizado
				|| matricula.Status == StatusMatricula.Concluido)
			{
				logger.LogInformation(
					"[PagamentoConfirmadoIntegrationHandler] Matrícula {MatriculaId} já no status {Status}. Ignorado (idempotência).",
					message.MatriculaCursoId, matricula.Status);
				return;
			}

			var comando = new AlterarStatusMatriculaCommand(
				message.MatriculaCursoId,
				StatusMatricula.PagamentoRealizado);

			var sucesso = await mediatorHandler.EnviarComandoRaiz(comando);

			if (sucesso)
				logger.LogInformation(
					"[PagamentoConfirmadoIntegrationHandler] Matrícula {MatriculaId} ativada (PagamentoRealizado).",
					message.MatriculaCursoId);
			else
				logger.LogWarning(
					"[PagamentoConfirmadoIntegrationHandler] Falha ao ativar matrícula {MatriculaId}.",
					message.MatriculaCursoId);
		}
		catch (Exception ex)
		{
			logger.LogError(ex,
				"[PagamentoConfirmadoIntegrationHandler] Erro ao processar evento para matrícula {MatriculaId}: {Message}",
				message.MatriculaCursoId, ex.Message);
			throw;
		}
	}
}
