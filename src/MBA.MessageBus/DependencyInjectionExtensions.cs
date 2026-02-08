using Microsoft.Extensions.DependencyInjection;

namespace MBA.MessageBus;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, string connection)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connection);

        services.AddSingleton<IMessageBus>(_ => new MessageBus(connection));

        return services;
    }
}