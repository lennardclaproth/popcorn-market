using MediatR;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.SharedKernel.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, AggregateRoot aggregateRoot,
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
}
