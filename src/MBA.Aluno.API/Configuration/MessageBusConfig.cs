using MBA.Aluno.API.Services;
using MBA.MessageBus;
using MBA.Core.Utils;

namespace MBA.Aluno.API.Configuration;

public static class MessageBusConfig
{
	public static void AddMessageBusConfiguration(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
			.AddHostedService<CadastroAlunoIntegrationHandler>()
			.AddHostedService<PagamentoConfirmadoIntegrationHandler>();
	}
}