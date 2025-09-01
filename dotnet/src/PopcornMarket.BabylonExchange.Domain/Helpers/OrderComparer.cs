using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Domain.Helpers;

/// <summary>
/// Provides custom sorting for orders (handles both market and limit orders).
/// </summary>
public class OrderComparer : IComparer<Order>
{
    private readonly bool _isBuyOrder;

    public OrderComparer(bool isBuyOrder) => _isBuyOrder = isBuyOrder;

    public int Compare(Order? x, Order? y)
    {
        if (x == null || y == null) return 0;

        // 1. Market orders always come before limit orders
        if (x.OrderType == OrderType.MarketOrder && y.OrderType != OrderType.MarketOrder)
            return -1;
        if (y.OrderType == OrderType.MarketOrder && x.OrderType != OrderType.MarketOrder)
            return 1;

        // 2. If both are market orders → FIFO by timestamp
        if (x.OrderType == OrderType.MarketOrder && y.OrderType == OrderType.MarketOrder)
            return x.PlacedTimestamp.CompareTo(y.PlacedTimestamp);

        // 3. Both are limit orders → price priority then FIFO
        int priceComparison = _isBuyOrder
            ? y.Price.CompareTo(x.Price)   // Higher price first for buys
            : x.Price.CompareTo(y.Price);  // Lower price first for sells

        return priceComparison == 0
            ? x.PlacedTimestamp.CompareTo(y.PlacedTimestamp)
            : priceComparison;
    }
}
