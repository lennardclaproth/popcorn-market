using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class OrderBookCache
{
    private readonly ConcurrentDictionary<string, CachedOrderBook> _cache = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _evictionTimeout;
    private readonly ILogger<OrderBookCache> _logger;
    private readonly int _window = 10;
    public OrderBookCache(IServiceScopeFactory scopeFactory, TimeSpan evictionTimeout, ILogger<OrderBookCache> logger)
    {
        _scopeFactory = scopeFactory;
        _evictionTimeout = evictionTimeout;
        _logger = logger;
    }

    public async Task<OrderBook?> Get(string ticker)
    {
        // Efficiently update orderbook cache as well. Use a window of orders 
        // you keep in memory so that the memory does not get overloaded.
        if (_cache.TryGetValue(ticker, out var cached))
        {
            _logger.LogInformation("Order book for ticker {Ticker} found in cache.", ticker);
            cached.Touch();
            return cached.OrderBook;
        }

        using var scope = _scopeFactory.CreateScope();
        var orderBookRepository = scope.ServiceProvider.GetRequiredService<IOrderBookRepository>();

        var book = await orderBookRepository.GetByStockSymbolIncludingPendingOrders(ticker, 1, _window);
        if (book == null) return null;

        // Initialize cursors based on what’s already loaded
        var lastBuyOrder = book.BuyOrders.LastOrDefault();
        var lastSellOrder = book.SellOrders.LastOrDefault();

        var lastBuyCursor = lastBuyOrder != null
            ? new OrderBookCursor(lastBuyOrder.Price, lastBuyOrder.PlacedTimestamp)
            : new OrderBookCursor(null, null);

        var lastSellCursor = lastSellOrder != null
            ? new OrderBookCursor(lastSellOrder.Price, lastSellOrder.PlacedTimestamp)
            : new OrderBookCursor(null, null);

        var newCached = new CachedOrderBook(book, lastBuyCursor, lastSellCursor);
        _cache[ticker] = newCached;
        _logger.LogInformation("Order book for ticker {Ticker} loaded from database and added to cache.", ticker);

        return book;
    }

    public void EvictStale()
    {
        var now = DateTimeOffset.UtcNow;
        var evictedCount = 0;
        var totalDomainEventsCleared = 0;
        
        foreach (var kv in _cache)
        {
            if (now - kv.Value.LastAccessed > _evictionTimeout)
            {
                var domainEventCount = kv.Value.OrderBook.DomainEvents.Count;
                if (domainEventCount > 0)
                {
                    _logger.LogWarning("Evicting stale order book for ticker {Ticker} with {DomainEventCount} unprocessed domain events. This may indicate a processing issue.", 
                        kv.Key, domainEventCount);
                    totalDomainEventsCleared += domainEventCount;
                }
                
                _logger.LogInformation("Evicting stale order book for ticker {Ticker}", kv.Key);
                _cache.TryRemove(kv.Key, out _);
                evictedCount++;
            }
        }
        
        if (evictedCount > 0)
        {
            _logger.LogInformation("Evicted {EvictedCount} stale order books, cleared {TotalDomainEvents} unprocessed domain events", 
                evictedCount, totalDomainEventsCleared);
        }
    }

    /// <summary>
    /// Fills the order book with orders from the cold storage.
    /// </summary>
    /// <param name="orderBook"></param>
    /// <param name="orderSide"></param>
    /// <returns>true if there were still orders false if there weren't</returns>
    public async Task<bool> FillHotOrders(OrderBook orderBook, OrderSide orderSide)
    {
        if (!_cache.TryGetValue(orderBook.StockSymbol, out var cached))
        {
            _logger.LogWarning("Order book for {Ticker} not found in cache during FillHotOrders.", orderBook.StockSymbol);
            return false;
        }

        var cursor = orderSide == OrderSide.Buy
            ? cached.LastSellCursor
            : cached.LastBuyCursor;
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var coldOrders = await repository.GetPendingOrdersByTicker(
            orderBook.StockSymbol, orderSide,
            cursor.LastPrice, cursor.LastPlacedTimestamp, _window);

        if (coldOrders.Count == 0)
        {
            _logger.LogInformation("No more cold orders for {Ticker} ({Side}).", orderBook.StockSymbol, orderSide);
            return false;
        }

        foreach (var coldOrder in coldOrders)
        {
            orderBook.RestOrder(coldOrder);
        }

        var last = coldOrders.Last();
        if (orderSide == OrderSide.Buy)
            cached.LastSellCursor = new OrderBookCursor(last.Price, last.PlacedTimestamp);
        else
            cached.LastBuyCursor = new OrderBookCursor(last.Price, last.PlacedTimestamp);

        _logger.LogInformation("Loaded {Count} cold orders for {Ticker} ({Side}).", coldOrders.Count, orderBook.StockSymbol, orderSide);
        return true;
    }

    public Task EvictColdOrders(OrderBook orderBook)
    {
        var coldOrders = orderBook.OrdersOutsideWindow(_window);
        if (coldOrders.Count == 0) return Task.CompletedTask;
        _logger.LogInformation("Evicting cold orders for order book with ticker {Ticker}, found {OrderCount} orders.", orderBook.StockSymbol, coldOrders.Count);
        orderBook.EvictOrders(coldOrders);
        return Task.CompletedTask;
    }
}
