using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace PopcornMarket.SharedKernel.Abstractions;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent
{ }
