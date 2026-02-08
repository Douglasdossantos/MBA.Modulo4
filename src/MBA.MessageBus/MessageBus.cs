using EasyNetQ;
using EasyNetQ.Persistent;
using MBA.Core.Messages.Integration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace MBA.MessageBus;

public sealed class MessageBus(string connectionString) : IMessageBus
{
    private readonly string _connectionString = !string.IsNullOrWhiteSpace(connectionString)
        ? connectionString
        : throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

    private readonly object _sync = new();
    private ServiceProvider? _serviceProvider;
    private IBus? _bus;
    private IAdvancedBus? _advancedBus;
    private bool _disposed;

    public bool IsConnected =>
        _advancedBus is not null
        && _advancedBus.GetConnectionStatus(PersistentConnectionType.Producer).State
        == PersistentConnectionState.Connected
        && _advancedBus.GetConnectionStatus(PersistentConnectionType.Consumer).State
        == PersistentConnectionState.Connected;
    public IAdvancedBus AdvancedBus => _advancedBus ?? throw new InvalidOperationException("Message bus is not connected.");

    public void Publish<T>(T message, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(message);
        EnsureConnected();
        _bus!.PubSub.PublishAsync(message, cancellationToken).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : IntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(message);
        EnsureConnected();
        await _bus!.PubSub.PublishAsync(message, cancellationToken);
    }

    public SubscriptionResult Subscribe<T>(
        string subscriptionId,
        Action<T> onMessage,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        ArgumentNullException.ThrowIfNull(onMessage);
        EnsureConnected();
        return _bus!.PubSub.SubscribeAsync(subscriptionId, onMessage, cancellationToken).GetAwaiter().GetResult();
    }

    public Task<SubscriptionResult> SubscribeAsync<T>(
        string subscriptionId,
        Func<T, Task> onMessage,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        ArgumentNullException.ThrowIfNull(onMessage);
        EnsureConnected();
        return _bus!.PubSub.SubscribeAsync(subscriptionId, onMessage, cancellationToken);
    }

    public TResponse Request<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureConnected();
        return _bus!.Rpc.RequestAsync<TRequest, TResponse>(request, cancellationToken).GetAwaiter().GetResult();
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureConnected();
        return await _bus!.Rpc.RequestAsync<TRequest, TResponse>(request, cancellationToken);
    }

    public IAsyncDisposable Respond<TRequest, TResponse>(
        Func<TRequest, TResponse> responder,
        CancellationToken cancellationToken = default
    )
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage
    {
        ArgumentNullException.ThrowIfNull(responder);
        EnsureConnected();
        return _bus!.Rpc.RespondAsync(responder, cancellationToken).GetAwaiter().GetResult();
    }

    public Task<IAsyncDisposable> RespondAsync<TRequest, TResponse>(
        Func<TRequest, Task<TResponse>> responder,
        CancellationToken cancellationToken = default
    )
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage
    {
        ArgumentNullException.ThrowIfNull(responder);
        EnsureConnected();
        return _bus!.Rpc.RespondAsync(responder, cancellationToken);
    }

    private void EnsureConnected()
    {
        ThrowIfDisposed();
        if (IsConnected) return;

        lock (_sync)
        {
            if (IsConnected) return;

            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() => ConnectAsync(CancellationToken.None).GetAwaiter().GetResult());
        }
    }

    private void OnDisconnect(object? sender, DisconnectedEventArgs e)
    {
        if (_disposed) return;

        var policy = Policy.Handle<EasyNetQException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetryForever(_ => TimeSpan.FromSeconds(5));

        policy.Execute(EnsureConnected);
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_advancedBus is not null)
        {
            _advancedBus.Disconnected -= OnDisconnect;
        }

        await DisposeBusAsync();
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await DisposeBusAsync();

        var services = new ServiceCollection();
        RabbitHutch.AddEasyNetQ(services, _connectionString);

        _serviceProvider = services.BuildServiceProvider();
        _bus = _serviceProvider.GetRequiredService<IBus>();
        _advancedBus = _bus.Advanced;

        _advancedBus.Disconnected -= OnDisconnect;
        _advancedBus.Disconnected += OnDisconnect;

        await _advancedBus.EnsureConnectedAsync(PersistentConnectionType.Producer, cancellationToken);
        await _advancedBus.EnsureConnectedAsync(PersistentConnectionType.Consumer, cancellationToken);
    }

    private async Task DisposeBusAsync()
    {
        if (_advancedBus is not null)
        {
            _advancedBus.Disconnected -= OnDisconnect;
        }

        _bus = null;

        if (_serviceProvider is not null)
        {
            await _serviceProvider.DisposeAsync();
            _serviceProvider = null;
        }

        _advancedBus = null;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(MessageBus));
        }
    }
}