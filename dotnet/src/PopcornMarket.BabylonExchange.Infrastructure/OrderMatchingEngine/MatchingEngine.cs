using System.Diagnostics;
using System.Threading.Channels;
using Ardalis.GuardClauses;
using Elastic.Apm.Api;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
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
    private readonly ITracer _tracer;

    public MatchingEngine(Channel<Order> channel, IServiceScopeFactory scopeFactory, OrderBookCache cache, ILogger<MatchingEngine> logger, IDomainEventQueue eventQueue, ITracer tracer)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
        _eventQueue = eventQueue;
        _tracer = tracer;
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
            await ProcessOrder(order, stoppingToken);
        }
    }

    private async Task ProcessOrder(Order order, CancellationToken stoppingToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        _logger.LogDebug("Processing order {OrderId} for {StockSymbol}", order.Id, order.StockSymbol);

        var transaction = _tracer.StartTransaction(nameof(ProcessOrder), nameof(MatchingEngine));
        OrderBook? orderBook = null;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            orderBook = await _cache.Get(order.StockSymbol);
            Guard.Against.Null(orderBook, nameof(orderBook));
            var matchResult = orderBook.MatchOrder(order);
            if (matchResult.IsFailure && matchResult.Error.Equals(OrderBookErrors.OrderBookMatchOrderCacheMiss))
            {
                _logger.LogDebug(
                    "Order {OrderId} for {StockSymbol} had a match order miss in the order book cache. Checking cold storage.",
                    order.Id, order.StockSymbol);
                bool hasMoreColdOrders = true;
                bool fullyMatched = false;
                int safetyCounter = 0;

                while (hasMoreColdOrders && !fullyMatched && safetyCounter++ < 20)
                {
                    var result = orderBook.MatchOrder(order);

                    if (result.IsSuccess)
                    {
                        break;
                    }

                    if (result.Error.Equals(OrderBookErrors.OrderBookMatchOrderCacheMiss))
                    {
                        hasMoreColdOrders = order.OrderSide == OrderSide.Buy
                            ? await _cache.FillHotOrders(orderBook, OrderSide.Sell)
                            : await _cache.FillHotOrders(orderBook, OrderSide.Buy);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _logger.LogDebug("Memory usage at matching engine after match order: {Memory} MB",
                GC.GetTotalMemory(false) / (1024 * 1024));

            var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            await _cache.EvictColdOrders(orderBook);
            _logger.LogDebug(
                "Order {OrderId} for {StockSymbol} processed in {ElapsedTimeMs} ms by the matching engine", order.Id,
                order.StockSymbol, elapsedTimeMs);
            _logger.LogDebug("Dispatching {EventCount} domain events for OrderBook {StockSymbol}",
                order.StockSymbol, orderBook.DomainEvents.Count);
            _logger.LogDebug("Memory usage at matching engine before dispatching match order: {Memory} MB",
                GC.GetTotalMemory(false) / (1024 * 1024));
            await mediator.DispatchDomainEventsToQueueAsync(orderBook, _eventQueue, stoppingToken);
            _logger.LogDebug("Memory usage at matching engine after dispatching match order: {Memory} MB",
                GC.GetTotalMemory(false) / (1024 * 1024));
            elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            _logger.LogDebug(
                "Domain events for OrderBook {StockSymbol} have been dispatched total processing time {ElapsedTimeMs}",
                order.StockSymbol, elapsedTimeMs);
        }
        catch (Exception ex)
        {
            var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            _logger.LogError(ex,
                "Failed to process order {OrderId} for {StockSymbol} after {ElapsedTimeMs} ms. Order will be skipped to prevent blocking.",
                order.Id, order.StockSymbol, elapsedTimeMs);

            if (orderBook != null)
            {
                orderBook.ClearDomainEvents();
            }

            transaction.CaptureException(ex);
        }
        finally
        {
            transaction.End();
        }
    }
}
