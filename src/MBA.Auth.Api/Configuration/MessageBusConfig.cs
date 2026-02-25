using MBA.Core.Utils;
using MBA.MessageBus;

namespace MBA.Auth.Api.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));            
        }
    }
}
