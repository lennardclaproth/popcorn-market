using System.Diagnostics;
using System.Threading.Channels;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.SharedKernel.Extensions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class MatchingEngine : BackgroundService
{
    private readonly Channel<Order> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OrderBookCache _cache;
    private readonly ILogger<MatchingEngine> _logger;

    public MatchingEngine(Channel<Order> channel, IServiceScopeFactory scopeFactory, OrderBookCache cache, ILogger<MatchingEngine> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Lazily loads an orderBook from the database with it's currentState and
    /// tries to execute the order. If the order is not able to be executed it adds
    /// the order to the orderBook (resting orders). When the orderBook gets accessed
    /// it also updates the LastAccessed date so that stale orderBooks can be evicted.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var order in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            var startTime = Stopwatch.GetTimestamp();
            _logger.LogInformation("Processing order {OrderId} for {StockSymbol}", order.Id, order.StockSymbol);
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var orderBook = await _cache.Get(order.StockSymbol);
            Guard.Against.Null(orderBook, nameof(orderBook));

            orderBook.MatchOrder(order);
            var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            _logger.LogInformation("Order {OrderId} for {StockSymbol} processed in {ElapsedTimeMs} ms by the matching engine", order.Id, order.StockSymbol, elapsedTimeMs);

            _logger.LogInformation("Dispatching domain events for OrderBook {StockSymbol}", order.StockSymbol);

            await mediator.DispatchDomainEventsAsync(orderBook, stoppingToken);
            await unitOfWork.SaveChangesAsync(stoppingToken);

            elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            _logger.LogInformation("Domain events for OrderBook {StockSymbol} have been dispatched total processing time {ElapsedTimeMs}", order.StockSymbol, elapsedTimeMs);
        }
    }
}
