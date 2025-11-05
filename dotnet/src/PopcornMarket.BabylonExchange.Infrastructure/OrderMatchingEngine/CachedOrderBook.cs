using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class CachedOrderBook
{
    public OrderBook OrderBook { get;}
    public DateTimeOffset LastAccessed { get; set; }
    public CachedOrderBook(OrderBook orderBook)
    {
        OrderBook = orderBook;
        LastAccessed = DateTimeOffset.Now;
    }

    public void Touch() => LastAccessed = DateTimeOffset.UtcNow;
}
