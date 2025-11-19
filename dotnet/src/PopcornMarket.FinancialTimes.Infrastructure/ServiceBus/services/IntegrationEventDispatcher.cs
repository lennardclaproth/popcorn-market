using PopcornMarket.FinancialTimes.Application.Abstractions;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.SharedKernel.Exceptions;

namespace PopcornMarket.FinancialTimes.Infrastructure.ServiceBus.services;
internal sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private readonly IOutbox _outbox;
    private readonly IProducer _producer;

    public IntegrationEventDispatcher(IOutbox outbox, IProducer producer)
    {
        _outbox = outbox;
        _producer = producer;
    }

    public async Task Dispatch<T>(IIntegrationEvent<T> integrationEvent, CancellationToken ct = default)
    {
        await _producer.Produce(integrationEvent.Topic, integrationEvent.Payload, ct);
    }

    public async Task DispatchToOutbox<T>(IIntegrationEvent<T> integrationEvent, CancellationToken ct = default)
    {
        if (integrationEvent.Payload == null)
        {
            throw new RequiredPropertyIsNullException($"{nameof(integrationEvent.Payload)} cannot be null.");
        }
        await _outbox.EnqueueAsync(integrationEvent.Topic, integrationEvent.Payload, cancellationToken: ct);
    }
}
