using System.Diagnostics;
using System.Threading.Channels;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.SharedKernel.Abstractions;
using PopcornMarket.SharedKernel.Extensions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class MatchingEngine : BackgroundService
{
    private readonly Channel<Order> _channel;
    private readonly IDomainEventQueue _eventQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OrderBookCache _cache;
    private readonly ILogger<MatchingEngine> _logger;

    public MatchingEngine(Channel<Order> channel, IServiceScopeFactory scopeFactory, OrderBookCache cache, ILogger<MatchingEngine> logger, IDomainEventQueue eventQueue)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
        _eventQueue = eventQueue;
    }

    /// <summary>
    /// Lazily loads an orderBook from the database with it's currentState and
    /// tries to execute the order. If the order is not able to be executed it adds
    /// the order to the orderBook (resting orders). When the orderBook gets accessed
    /// it also updates the LastAccessed date so that stale orderBooks can be evicted.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var order in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await ProcessOrderSafely(order, stoppingToken);
        }
    }

    private async Task ProcessOrderSafely(Order order, CancellationToken stoppingToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        _logger.LogInformation("Processing order {OrderId} for {StockSymbol}", order.Id, order.StockSymbol);
        
        OrderBook? orderBook = null;
        
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            orderBook = await _cache.Get(order.StockSymbol);
            Guard.Against.Null(orderBook, nameof(orderBook));

            // Store events count before matching to track what was added
            var eventsBeforeMatching = orderBook.DomainEvents.Count;
            _logger.LogInformation("Domain events count: {Count}", eventsBeforeMatching);

            try
            {
                orderBook.MatchOrder(order);
                var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
                _logger.LogInformation("Order {OrderId} for {StockSymbol} processed in {ElapsedTimeMs} ms by the matching engine", order.Id, order.StockSymbol, elapsedTimeMs);
                _logger.LogInformation("Dispatching domain events for OrderBook {StockSymbol}", order.StockSymbol);
                await mediator.DispatchDomainEventsToQueueAsync(orderBook, _eventQueue, stoppingToken);
                elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
                _logger.LogInformation("Domain events for OrderBook {StockSymbol} have been dispatched total processing time {ElapsedTimeMs}", order.StockSymbol, elapsedTimeMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during order processing or event dispatching for order {OrderId} for {StockSymbol}. Clearing domain events to prevent memory leak.", order.Id, order.StockSymbol);
                
                // Critical: Clear domain events that were generated during the failed processing
                // This prevents memory leaks from accumulated events that were never properly processed
                orderBook.ClearDomainEvents();
                
                // Re-throw to be handled by outer catch
                throw;
            }
        }
        catch (Exception ex)
        {
            var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            _logger.LogError(ex, "Failed to process order {OrderId} for {StockSymbol} after {ElapsedTimeMs} ms. Order will be skipped to prevent blocking.", 
                order.Id, order.StockSymbol, elapsedTimeMs);
                
            // Ensure domain events are cleared even if we couldn't get the orderBook earlier
            if (orderBook != null)
            {
                try
                {
                    orderBook.ClearDomainEvents();
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to cleanup domain events for {StockSymbol}", order.StockSymbol);
                }
            }
            
            // Continue processing other orders - don't let one failed order stop the entire engine
        }
    }
}
