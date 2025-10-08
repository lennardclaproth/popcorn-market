using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.EventHandlers;
internal sealed class OrderPartiallyFilledHandler : IDomainEventHandler<OrderPartiallyFilled>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderPartiallyFilledHandler> _logger;

    public OrderPartiallyFilledHandler(IOrderRepository orderRepository, ILogger<OrderPartiallyFilledHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task Handle(OrderPartiallyFilled notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling OrderPartiallyFilled event for OrderId: {OrderId} to persist changes", notification.Id);
        var startTime = Stopwatch.GetTimestamp();
        var order = await _orderRepository.GetById(notification.Id);
        Guard.Against.Null(order);
        order.PartiallyFulfillOrder(notification.TradePrice, notification.RemainingQuantity, notification.FulfilledAt);
        await _orderRepository.UpdateEntity(order);
        _logger.LogInformation("Order with OrderId: {OrderId} partial fill has been persisted in {ElapsedTimeMs} ms", notification.Id, Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);
    }
}
