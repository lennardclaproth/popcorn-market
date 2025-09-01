using System.Threading.Channels;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.SharedKernel.Extensions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderExecutionEngine;

public class MatchingEngine : BackgroundService
{
    private readonly Channel<Order> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<string, CachedOrderBook> _loadedOrderBooks = new();

    public MatchingEngine(Channel<Order> channel, IServiceScopeFactory scopeFactory)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
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
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IOrderBookRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            if (!_loadedOrderBooks.ContainsKey(order.StockSymbol))
            {
                var book = await repo.GetByTickerIncludingPendingOrders(order.StockSymbol);
                Guard.Against.Null(book, nameof(book));
                _loadedOrderBooks.Add(book.Ticker, new CachedOrderBook(book));
            }
            var orderBookCache = _loadedOrderBooks[order.StockSymbol];
            orderBookCache.LastAccessed = DateTimeOffset.Now;
            orderBookCache.OrderBook.MatchOrder(order);
            await mediator.DispatchDomainEventsAsync(orderBookCache.OrderBook, stoppingToken);
        }
    }
}
