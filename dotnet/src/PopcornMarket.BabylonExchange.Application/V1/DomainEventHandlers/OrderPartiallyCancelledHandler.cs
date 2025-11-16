using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.DomainEventHandlers;
internal sealed class OrderPartiallyCancelledHandler : IDomainEventHandler<OrderPartiallyCancelled>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IIntegrationEventDispatcher _integrationEventDispatcher;
    private readonly ILogger<OrderPartiallyCancelledHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrderPartiallyCancelledHandler(IOrderRepository orderRepository,
        ILogger<OrderPartiallyCancelledHandler> logger,
        IIntegrationEventDispatcher integrationEventDispatcher, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _integrationEventDispatcher = integrationEventDispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(OrderPartiallyCancelled notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling OrderPartiallyCancelled event for OrderId: {OrderId} to persist changes", notification.OrderId);
        var startTime = Stopwatch.GetTimestamp();
        var order = await _orderRepository.GetById(notification.OrderId);
        Guard.Against.Null(order, nameof(order));
        order.PartiallyCancelOrder(notification.Reason, notification.RemainingQuantity, notification.CancelledAt);
        await _orderRepository.UpdateEntity(order);
        var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
        _logger.LogDebug("Order with OrderId: {OrderId} partial cancellation has been persisted in {ElapsedTimeMs}", notification.OrderId, elapsedTimeMs);

        var payload = new OrderPartiallyCancelledPayload
        {
            OrderId = order.Id,
            Reason = notification.Reason,
            CancelledAt = notification.CancelledAt,
            RemainingQuantity = notification.RemainingQuantity
        };
        var integrationEvent = new OrderPartiallyCancelledIntegrationEvent(payload);

        await _integrationEventDispatcher.DispatchToOutbox(integrationEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
