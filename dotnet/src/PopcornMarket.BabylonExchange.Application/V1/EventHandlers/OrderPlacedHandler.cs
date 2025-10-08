using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.EventHandlers;

public class OrderPlacedHandler : IDomainEventHandler<OrderPlaced>
{
    private readonly IOrderQueue _orderQueue;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderPlacedHandler> _logger;

    public OrderPlacedHandler(IOrderQueue orderQueue, IOrderRepository orderRepository, ILogger<OrderPlacedHandler> logger)
    {
        _orderQueue = orderQueue;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Fetches the order from the database and places it in the queue. 
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Handle(OrderPlaced notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling OrderPlaced event for OrderId: {OrderId} to enqueue the order", notification.OrderId);
        var startTime = Stopwatch.GetTimestamp();
        var order = await _orderRepository.GetById(notification.OrderId);
        Guard.Against.Null(order, nameof(order));
        await _orderQueue.Enqueue(order);
        _logger.LogInformation("Order with OrderId: {OrderId} has been enqueued in {ElapsedTimeMs} ms", notification.OrderId, Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);
    }
}
