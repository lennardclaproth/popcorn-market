using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.EventHandlers;
internal sealed class TradeExecutedHandler : IDomainEventHandler<TradeExecuted>
{
    private readonly ITradeRepository _tradeRepository;
    private readonly ILogger<TradeExecutedHandler> _logger;
    private readonly IListingRepository _listingRepository;

    public TradeExecutedHandler(ITradeRepository tradeRepository, ILogger<TradeExecutedHandler> logger, IListingRepository listingRepository)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
        _listingRepository = listingRepository;
    }

    public async Task Handle(TradeExecuted notification, CancellationToken cancellationToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        var listing = await _listingRepository.GetByStockSymbol(notification.StockSymbol);
        Guard.Against.Null(listing, nameof(listing), $"Listing with StockSymbol: {notification.StockSymbol} not found. Trade cannot be created.");

        var trade = Trade.Create(notification.BuyOrderId,
                                 notification.SellOrderId,
                                 notification.StockSymbol,
                                 notification.TradePrice,
                                 notification.TradeQuantity,
                                 notification.ExecutedAt);

        await _tradeRepository.AddEntity(trade);

        listing.ApplyTrade(trade.Price, trade.Quantity, trade.ExecutedAt);
        await _listingRepository.UpdateEntity(listing);

        _logger.LogInformation("Trade for Listing: '{Symbol}' has been persisted in {ElapsedTime} ms",
                               listing.StockSymbol,
                               Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);
    }
}
