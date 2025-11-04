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
            try
            {
                _logger.LogInformation("Starting cache eviction.");
                _cache.EvictStale();
                _logger.LogInformation("Cache eviction cycle complete. Waiting for next cycle.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cache eviction cycle.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
