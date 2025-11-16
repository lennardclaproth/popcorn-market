using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.ServiceBus.Models;

namespace PopcornMarket.ServiceBus.Services;
internal sealed class EfCoreOutboxService<TDbContext> : IOutbox
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public EfCoreOutboxService(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnqueueAsync(string topic, object payload, string? key = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);

        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Topic = topic,
            Payload = json,
            Key = key,
            CreatedAtUtc = DateTime.UtcNow,
            Attempts = 0
        };

        await _dbContext.Set<OutboxMessage>().AddAsync(message, cancellationToken);
        // Important: SaveChanges should happen in the *same transaction* as your domain changes.
    }

    public async Task<List<OutboxMessage>> GetUnprocessedBatchAsync(int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedAtUtc == null && m.Attempts < 10)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        message.ProcessedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        message.Attempts++;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
