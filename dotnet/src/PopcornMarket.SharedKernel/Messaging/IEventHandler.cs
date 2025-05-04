// ReSharper disable UnusedType.Global

using System.Diagnostics.CodeAnalysis;

namespace PopcornMarket.SharedKernel.Messaging;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
