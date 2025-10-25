using System.Text.Json;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Persistence.Context;
using PopcornMarket.BabylonExchange.Persistence.Entities;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Services;
internal sealed class EfCoreOutboxService : IOutboxService
{
    private readonly BabylonExchangeDbContext _dbContext;

    public EfCoreOutboxService(BabylonExchangeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Add(IDomainEvent @event, CancellationToken ct = default)
    {
        var message = new OutboxMessage
        {
            OccurredOnUtc = DateTime.UtcNow,
            Type = @event.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(@event, @event.GetType())
        };
        _dbContext.OutboxMessages.Add(message);
        return _dbContext.SaveChangesAsync(ct);
    }
}
