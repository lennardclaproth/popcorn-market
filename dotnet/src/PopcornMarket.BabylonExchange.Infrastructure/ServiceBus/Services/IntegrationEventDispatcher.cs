using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Services;

internal sealed class OutboxService : IOutboxService
{
    private readonly IOutbox _outbox;

    public OutboxService(IOutbox outbox)
    {
        _outbox = outbox;
    }

    public async Task Add(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        await _outbox.EnqueueAsync("test", domainEvent, cancellationToken: ct);
    }
}
