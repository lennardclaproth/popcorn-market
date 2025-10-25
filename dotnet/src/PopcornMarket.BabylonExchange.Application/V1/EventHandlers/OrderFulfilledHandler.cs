using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.EventHandlers;
internal sealed class OrderFulfilledHandler : IDomainEventHandler<OrderFulfilled>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOutboxService _outboxService;
    private readonly ILogger<OrderFulfilledHandler> _logger;

    public OrderFulfilledHandler(IOrderRepository orderRepository,
        ILogger<OrderFulfilledHandler> logger,
        IOutboxService outboxService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _outboxService = outboxService;
    }

#warning Lennard Claproth [05/09/2025] we potentially set the tradeprice wrong here, this should be an average of the executed trades. 
    public async Task Handle(OrderFulfilled notification, CancellationToken cancellationToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        _logger.LogInformation("Handling OrderFulfilled event for OrderId: {OrderId} to persist changes", notification.Id);
        var order = await _orderRepository.GetById(notification.Id);
        Guard.Against.Null(order);
        order.FulfillOrder(notification.TradePrice, notification.FulfilledAt);
        await _orderRepository.UpdateEntity(order);
        var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
        _logger.LogInformation("Order with OrderId: {OrderId} fulfillment has been persisted in {ElapsedTimeMs}", notification.Id, elapsedTimeMs);
        await _outboxService.Add(notification, cancellationToken);
    }
}
