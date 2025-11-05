using System.Text.Json;
using MediatR;
using PopcornMarket.SharedKernel.Abstractions;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.SharedKernel.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEvents(this IMediator mediator, AggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default)
    {
        if (aggregateRoot.DomainEvents.Count == 0)
        {
            return;
        }

        var domainEvents = aggregateRoot.DomainEvents.ToList();
        aggregateRoot.ClearDomainEvents();
        
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }
    }
    public static async Task DispatchDomainEventsToQueue(
        this IMediator mediator,
        AggregateRoot aggregateRoot,
        IDomainEventQueue queue,
        CancellationToken cancellationToken = default)
    {
        if (aggregateRoot.DomainEvents.Count == 0)
            return;

        var domainEvents = aggregateRoot.DomainEvents.ToList();
        try
        {
            foreach (var domainEvent in domainEvents)
            {
                await queue.Enqueue(domainEvent, cancellationToken);
            }

            aggregateRoot.ClearDomainEvents();
        }
        catch
        {
            aggregateRoot.ClearDomainEvents();
            throw;
        }
    }
}
