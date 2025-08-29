using Ardalis.GuardClauses;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.EventHandlers;

public class OrderPlacedHandler : IDomainEventHandler<OrderPlaced>
{
    private readonly IOrderQueue _orderQueue;
    private readonly IOrderRepository _orderRepository;

    public OrderPlacedHandler(IOrderQueue orderQueue, IOrderRepository orderRepository)
    {
        _orderQueue = orderQueue;
        _orderRepository = orderRepository;
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
        var order = await _orderRepository.GetById(notification.OrderId);
        Guard.Against.Null(order, nameof(order));
        await _orderQueue.Enqueue(order);
    }
}
