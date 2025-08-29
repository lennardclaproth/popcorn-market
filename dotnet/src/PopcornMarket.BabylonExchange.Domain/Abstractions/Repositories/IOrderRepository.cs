using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Gets a list of pending buy orders. Groups them by market orders and limit orders.
    /// </summary>
    /// <param name="orderBookId"></param>
    /// <returns></returns>
    Task<Dictionary<OrderType, List<Order>>> GetPendingBuyOrders(Guid orderBookId);
    Task<Dictionary<OrderType, List<Order>>> GetPendingSellOrders(Guid orderBookId);
}
