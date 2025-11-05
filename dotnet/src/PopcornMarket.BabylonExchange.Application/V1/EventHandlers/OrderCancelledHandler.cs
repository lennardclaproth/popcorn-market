using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.EventHandlers;

internal sealed class OrderCancelledHandler : IDomainEventHandler<OrderCancelled>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOutboxService _outboxService;
    private readonly ILogger<OrderCancelledHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrderCancelledHandler(IOrderRepository orderRepository,
        ILogger<OrderCancelledHandler> logger,
        IOutboxService outboxService, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _outboxService = outboxService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(OrderCancelled notification, CancellationToken cancellationToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        _logger.LogDebug("Handling OrderCancelled event for OrderId: {OrderId} to persist changes", notification.OrderId);
        var order = await _orderRepository.GetById(notification.OrderId);
        Guard.Against.Null(order, nameof(order));
        order.CancelOrder(notification.Reason, notification.CancelledAt);
        await _orderRepository.UpdateEntity(order);
        var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
        _logger.LogDebug("Order with OrderId: {OrderId} cancellation has been persisted in {ElapsedTimeMs}", notification.OrderId, elapsedTimeMs);
        await _outboxService.Add(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
