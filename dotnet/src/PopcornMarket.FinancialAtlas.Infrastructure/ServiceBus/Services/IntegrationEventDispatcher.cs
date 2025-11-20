using PopcornMarket.FinancialAtlas.Application.Abstractions;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.SharedKernel.Exceptions;

namespace PopcornMarket.FinancialAtlas.Infrastructure.ServiceBus.Services;
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
