using System.Diagnostics.CodeAnalysis;

namespace PopcornMarket.SharedKernel.Abstractions;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public interface IDomainEventQueue
{
#pragma warning disable CA1716
    public Task Enqueue(IDomainEvent @event, CancellationToken cancellation);
#pragma warning restore CA1716
}
