using EasyNetQ;
using MBA.Core.Messages.Integration;

namespace MBA.MessageBus;

public interface IMessageBus : IDisposable, IAsyncDisposable
{
    bool IsConnected { get; }
    IAdvancedBus AdvancedBus { get; }

    void Publish<T>(T message, CancellationToken cancellationToken = default) where T : IntegrationEvent;

    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IntegrationEvent;

    SubscriptionResult Subscribe<T>(
        string subscriptionId,
        Action<T> onMessage,
        CancellationToken cancellationToken = default
    )
        where T : class;

    Task<SubscriptionResult> SubscribeAsync<T>(
        string subscriptionId,
        Func<T, Task> onMessage,
        CancellationToken cancellationToken = default
    ) where T : class;

    TResponse Request<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage;

    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage;

    IAsyncDisposable Respond<TRequest, TResponse>(
        Func<TRequest, TResponse> responder,
        CancellationToken cancellationToken = default
    )
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage;

    Task<IAsyncDisposable> RespondAsync<TRequest, TResponse>(
        Func<TRequest, Task<TResponse>> responder,
        CancellationToken cancellationToken = default
    )
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage;
}