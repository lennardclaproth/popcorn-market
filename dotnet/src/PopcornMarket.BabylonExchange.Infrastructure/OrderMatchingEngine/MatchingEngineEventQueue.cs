using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public sealed class MatchingEngineEventQueue : IDomainEventQueue
{
    private readonly Channel<IDomainEvent> _channel;

    public MatchingEngineEventQueue(Channel<IDomainEvent> channel)
    {
        _channel = channel;
    }

    public async Task Enqueue(IDomainEvent @event, CancellationToken cancellation)
    {
        await _channel.Writer.WriteAsync(@event, cancellation);
    }
}
