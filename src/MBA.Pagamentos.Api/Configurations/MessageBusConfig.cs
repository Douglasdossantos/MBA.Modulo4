using MBA.MessageBus;

namespace MBA.Pagamentos.Api.Configurations
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connection = configuration["MessageQueueConnection:MessageBus"];

            services.AddMessageBus(connection);
        }
    }
}
