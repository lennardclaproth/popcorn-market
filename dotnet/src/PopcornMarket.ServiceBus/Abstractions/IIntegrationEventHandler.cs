#pragma warning disable CA1711

namespace PopcornMarket.ServiceBus.Abstractions;

public interface IIntegrationEventHandler<in TPayload> : IIntegrationEventHandler where TPayload : class
{
    Task Handle(TPayload payload, CancellationToken cancellationToken);
}

public interface IIntegrationEventHandler
{
    Task Handle(object payload, CancellationToken cancellationToken);
}

public abstract class IntegrationEventHandler<TPayload> : IIntegrationEventHandler<TPayload> where TPayload : class
{
    public abstract Task Handle(TPayload payload, CancellationToken cancellationToken);

    async Task IIntegrationEventHandler.Handle(object payload, CancellationToken cancellationToken)
    {
        if (payload is not TPayload typed)
            throw new ArgumentException($"Expected payload of type {typeof(TPayload).Name} but got {payload?.GetType().Name}");

        await Handle(typed, cancellationToken);
    }
}

