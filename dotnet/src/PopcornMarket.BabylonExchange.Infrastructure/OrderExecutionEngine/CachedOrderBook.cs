using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderExecutionEngine;

public class CachedOrderBook
{
    public OrderBook OrderBook { get;}
    public DateTimeOffset LastAccessed { get; set; }

    public CachedOrderBook(OrderBook orderBook)
    {
        OrderBook = orderBook;
        LastAccessed = DateTimeOffset.Now;
    }
}
