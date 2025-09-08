using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class CacheEvictionService : BackgroundService
{
    private readonly OrderBookCache _cache;
    private readonly ILogger<CacheEvictionService> _logger;

    public CacheEvictionService(OrderBookCache cache, ILogger<CacheEvictionService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("Evicting stale order books from cache.");
            _cache.EvictStale();
            _logger.LogDebug("Eviction complete. Waiting for next cycle.");
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
