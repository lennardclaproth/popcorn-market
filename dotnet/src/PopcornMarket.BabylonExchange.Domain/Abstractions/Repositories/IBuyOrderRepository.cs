using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IBuyOrderRepository : IRepository<BuyOrder>
{
    /// <summary>
    /// Gets a list of pending buy orders. Groups them by market orders and limit orders.
    /// </summary>
    /// <param name="orderBookId"></param>
    /// <returns></returns>
    Task<Dictionary<OrderType, List<BuyOrder>>> GetPendingBuyOrders(Guid orderBookId);
}
