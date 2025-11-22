using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.FinancialTimes.Application.Abstractions;
public interface IIntegrationEventDispatcher
{
    public Task Dispatch<T>(IIntegrationEvent<T> integrationEvent, CancellationToken ct = default);
}
