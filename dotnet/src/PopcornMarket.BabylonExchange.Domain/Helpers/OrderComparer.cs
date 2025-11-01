using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Domain.Helpers;

/// <summary>
/// The OrderComparer provides a way to compare two orders based on their type, price, timestamp, and unique identifier.
/// A market order should always take precedence over a limit order. For limit orders, buy orders are prioritized by highest price first,
/// sell orders are prioritized by lowest price first. If prices are the same, orders are compared by their placement timestamp (FIFO).
/// </summary>
public class OrderComparer : IComparer<Order>
{
    private readonly bool _isBuyOrder;

    public OrderComparer(bool isBuyOrder) => _isBuyOrder = isBuyOrder;

    public int Compare(Order? x, Order? y)
    {
        // If x and y are the same object or both null, they are equal
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        // if x is a market order it should always take precedence over a limit order
        if (x.OrderType == OrderType.MarketOrder && y.OrderType != OrderType.MarketOrder)
            return -1;
        // if y is a market order it should always take precedence over a limit order
        if (y.OrderType == OrderType.MarketOrder && x.OrderType != OrderType.MarketOrder)
            return 1;

        // if both are market orders we should compare based on timestamp. FIFO + unique Id for fallback
        if (x.OrderType == OrderType.MarketOrder && y.OrderType == OrderType.MarketOrder)
        {
            int cmp = x.PlacedTimestamp.CompareTo(y.PlacedTimestamp);
            return cmp != 0 ? cmp : x.Id.CompareTo(y.Id);
        }

        // Both are limit orders, compare by price.
        int priceComparison = _isBuyOrder
            ? y.Price.CompareTo(x.Price)   // If the order is a buy order the highest price should go first.
            : x.Price.CompareTo(y.Price);  // If the order is a sell order the lowest price should go first.

        // If prices are different, return the comparison result.
        if (priceComparison != 0)
            return priceComparison;

        // If prices are the same, compare by timestamp (FIFO).
        int timeComparison = x.PlacedTimestamp.CompareTo(y.PlacedTimestamp);
        if (timeComparison != 0)
            return timeComparison;

        // If timestamps are also the same, use unique Id as a final tiebreaker.
        return x.Id.CompareTo(y.Id);
    }
}
