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
using PopcornMarket.SharedKernel.Abstractions;
using PopcornMarket.SharedKernel.Extensions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class MatchingEngine : BackgroundService
{
    private readonly Channel<Order> _channel;
    private readonly IDomainEventQueue _eventQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OrderBookCache _cache;
    private readonly ITracer _tracer;

    public MatchingEngine(Channel<Order> channel,
        IServiceScopeFactory scopeFactory,
        OrderBookCache cache,
        IDomainEventQueue eventQueue,
        ITracer tracer)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _cache = cache;
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
        var transaction = _tracer.StartTransaction($"{nameof(MatchingEngine)}.{nameof(ProcessOrder)}", nameof(BackgroundService));
        var orderMatched = true;
        var orderBook = await _cache.Get(order.StockSymbol);
        Guard.Against.Null(orderBook, nameof(orderBook));

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            while (order.RemainingQuantity > 0 && orderMatched)
            {
                orderMatched = orderBook.MatchOrder(order);
                await mediator.DispatchDomainEventsToQueue(orderBook, _eventQueue, stoppingToken);
            }

            if (orderMatched)
            {
                return;
            }

            if (order.OrderType == OrderType.LimitOrder)
            {
                orderBook.RestOrder(order);
                return;
            }

            if (order.OrderType == OrderType.MarketOrder)
            {
                orderBook.CancelOrder(order);
                await mediator.DispatchDomainEventsToQueue(orderBook, _eventQueue, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            orderBook.ClearDomainEvents();
            transaction.CaptureException(ex);
            throw;
        }
        finally
        {
            transaction.End();
        }
    }
}
