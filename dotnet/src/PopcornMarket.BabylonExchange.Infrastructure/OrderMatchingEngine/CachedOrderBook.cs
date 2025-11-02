using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class CachedOrderBook
{
    public OrderBook OrderBook { get;}
    public DateTimeOffset LastAccessed { get; set; }
    public OrderBookCursor LastBuyCursor { get; set; }
    public OrderBookCursor LastSellCursor { get; set; }
    public CachedOrderBook(OrderBook orderBook,
        OrderBookCursor lastBuyCursor,
        OrderBookCursor lastSellCursor)
    {
        OrderBook = orderBook;
        LastAccessed = DateTimeOffset.Now;
        LastBuyCursor = lastBuyCursor;
        LastSellCursor = lastSellCursor;
    }

    public void Touch() => LastAccessed = DateTimeOffset.UtcNow;
}
