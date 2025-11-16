using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.FinancialAtlas.Application.Abstractions;
public interface IIntegrationEventDispatcher
{
    public Task Dispatch<T>(IIntegrationEvent<T> integrationEvent, CancellationToken ct = default);
}
