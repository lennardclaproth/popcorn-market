using PopcornMarket.FinancialTimes.Application.Abstractions;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.FinancialTimes.Infrastructure.ServiceBus.services;
internal sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private readonly IProducer _producer;

    public IntegrationEventDispatcher(IProducer producer)
    {
        _producer = producer;
    }

    public async Task Dispatch<T>(IIntegrationEvent<T> integrationEvent, CancellationToken ct = default)
    {
        await _producer.Produce(integrationEvent.Topic, integrationEvent.Payload, ct);
    }
}
