using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.Abstractions;
public interface IIntegrationEventDispatcher
{
    public Task Dispatch<T>(IIntegrationEvent<T> integrationEvent, CancellationToken ct = default);
    public Task DispatchToOutbox<T>(IIntegrationEvent<T> integrationEvent, CancellationToken ct = default);
}
