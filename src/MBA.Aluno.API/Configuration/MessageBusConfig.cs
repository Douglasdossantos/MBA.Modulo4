using EasyNetQ;
using MBA.Aluno.API.Application.Commands;
using MBA.Aluno.API.Services;
using MBA.Core.Utils;
using MBA.MessageBus;

namespace MBA.Aluno.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                .AddHostedService<RegistroAlunoIntegrationHandler>();

            services.AddEasyNetQ(configuration.GetConnectionString("MessageBus"));


        }
    }
}
