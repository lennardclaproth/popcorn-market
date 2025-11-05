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
            cached.Touch();
            return cached.OrderBook;
        }

        using var scope = _scopeFactory.CreateScope();
        var orderBookRepository = scope.ServiceProvider.GetRequiredService<IOrderBookRepository>();

        var book = await orderBookRepository.GetByStockSymbolIncludingPendingOrders(ticker);
        if (book == null) return null;

        var newCached = new CachedOrderBook(book);
        _cache[ticker] = newCached;
        _logger.LogDebug("Cache miss occurred for ticker: {Ticker}, loaded order book from database into memory.", ticker);

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
                    throw new InvalidOperationException(
                        $"Orderbook still has orders to process, this indicates there was a processing issue. Ticker: {kv.Value.OrderBook.StockSymbol}, EventCount: {domainEventCount}");
                }
                
                _logger.LogDebug("Evicting stale order book for ticker {Ticker}", kv.Key);
                _cache.TryRemove(kv.Key, out _);
                evictedCount++;
            }
        }
        if (evictedCount > 0)
        {
            _logger.LogDebug("Evicted {EvictedCount} stale order books, cleared {TotalDomainEvents} unprocessed domain events", 
                evictedCount, totalDomainEventsCleared);
        }
    }
}
