using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.Abstractions;
public interface IOutboxService
{
    public Task Add(IDomainEvent domainEvent, CancellationToken ct = default);
}
