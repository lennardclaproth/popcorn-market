using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;

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
        if (_cache.TryGetValue(ticker, out var cached))
        {
            _logger.LogInformation("Order book for ticker {Ticker} found in cache.", ticker);
            cached.Touch();
            return cached.OrderBook;
        }

        using var scope = _scopeFactory.CreateScope();
        var orderBookRepository = scope.ServiceProvider.GetRequiredService<IOrderBookRepository>();

        var book = await orderBookRepository.GetByStockSymbolIncludingPendingOrdersAsNoTracking(ticker);
        if (book == null) return null;

        var newCached = new CachedOrderBook(book);
        _cache[ticker] = newCached;
        _logger.LogInformation("Order book for ticker {Ticker} loaded from database and added to cache.", ticker);

        return book;
    }

    public void EvictStale()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var kv in _cache)
        {
            if (now - kv.Value.LastAccessed > _evictionTimeout)
            {
                _logger.LogInformation("Evicting stale order book for ticker {Ticker}", kv.Key);
                _cache.TryRemove(kv.Key, out _);
            }
        }
    }
}
