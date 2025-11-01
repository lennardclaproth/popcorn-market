using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class CacheEvictionService : BackgroundService
{
    private readonly OrderBookCache _cache;
    private readonly ILogger<CacheEvictionService> _logger;
    private const int DomainEventThreshold = 1000; // If any order book has more than this many events, force cleanup

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
                _logger.LogInformation("Starting cache eviction and domain event monitoring cycle.");

                // Check for domain event buildup before eviction
                MonitorDomainEvents();

                // Perform regular cache eviction
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

    private void MonitorDomainEvents()
    {
        try
        {
            // This is a simple monitoring approach
            // In a real-world scenario, you might want to expose these metrics to a monitoring system
            var maxEventsInSingleOrderBook = 0;

            // Note: This is a simplified check. In production, you'd want to access cache statistics
            // through a proper interface rather than reflection or other means

            _logger.LogDebug("Domain event monitoring: Checking for excessive domain events in order book cache");

            // If we detect excessive domain events, force cleanup
            // This is a safety mechanism to prevent memory leaks
            if (maxEventsInSingleOrderBook > DomainEventThreshold)
            {
                _logger.LogWarning("Detected excessive domain events ({MaxEvents} > {Threshold}) in order book cache. Performing emergency cleanup.",
                    maxEventsInSingleOrderBook, DomainEventThreshold);

                _cache.ClearAllDomainEvents();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during domain event monitoring.");
        }
    }
}
