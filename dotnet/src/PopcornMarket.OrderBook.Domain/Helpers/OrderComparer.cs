using PopcornMarket.OrderBook.Domain.Entities;

namespace PopcornMarket.OrderBook.Domain.Helpers;

/// <summary>
/// Provides custom sorting for orders (for best price execution).
/// </summary>
public class OrderComparer : IComparer<Order>
{
    private readonly bool _isBuyOrder;
    public OrderComparer(bool isBuyOrder) => _isBuyOrder = isBuyOrder;
    
    public int Compare(Order? x, Order? y)
    {
        if (x == null || y == null) return 0;
        int priceComparison = _isBuyOrder ? y.Price.CompareTo(x.Price) : x.Price.CompareTo(y.Price);
        return priceComparison == 0 ? x.Timestamp.CompareTo(y.Timestamp) : priceComparison;
    }
}
