using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.DomainEventHandlers;
internal sealed class TradeExecutedHandler : IDomainEventHandler<TradeExecuted>
{
    private readonly ITradeRepository _tradeRepository;
    private readonly ILogger<TradeExecutedHandler> _logger;
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventDispatcher _integrationEventDispatcher;

    public TradeExecutedHandler(ITradeRepository tradeRepository, ILogger<TradeExecutedHandler> logger, IListingRepository listingRepository, IUnitOfWork unitOfWork, IIntegrationEventDispatcher integrationEventDispatcher)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
        _integrationEventDispatcher = integrationEventDispatcher;
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

        _logger.LogDebug("Trade for Listing: '{Symbol}' has been persisted in {ElapsedTime} ms",
                               listing.StockSymbol,
                               Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);

        var payload = new TradeExecutedPayload
        {
            BuyOrderId = notification.BuyOrderId,
            SellOrderId = notification.SellOrderId,
            StockSymbol = notification.StockSymbol,
            TradePrice = notification.TradePrice,
            TradeQuantity = notification.TradeQuantity,
            ExecutedAt = notification.ExecutedAt
        };
        var integrationEvent = new TradeExecutedIntegrationEvent(payload);
        await _integrationEventDispatcher.DispatchToOutbox(integrationEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
