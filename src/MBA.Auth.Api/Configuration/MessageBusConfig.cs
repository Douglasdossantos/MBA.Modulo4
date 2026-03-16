using MBA.MessageBus;
using MBA.Core.Utils;

namespace MBA.Auth.Api.Configuration
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
