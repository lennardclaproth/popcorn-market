using PopcornMarket.ServiceBus.Models;

namespace PopcornMarket.ServiceBus.Abstractions;
public interface IOutbox
{
    Task EnqueueAsync(string topic, object payload, string? key = null, CancellationToken cancellationToken = default);
    Task<List<OutboxMessage>> GetUnprocessedBatchAsync(int take, CancellationToken cancellationToken = default);
    Task MarkProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task MarkFailedAsync(OutboxMessage message, CancellationToken cancellationToken = default);
}

