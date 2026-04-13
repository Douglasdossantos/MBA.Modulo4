using MBA.MessageBus;
using MBA.Core.Utils;
using MBA.Bff.Api.Handlers;

namespace MBA.Bff.Api.Configuration;

public static class MessageBusConfig
{
	public static void AddMessageBusConfiguration(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
			.AddHostedService<AlterarStatusMatriculaIntegrationHandler>();
	}
}