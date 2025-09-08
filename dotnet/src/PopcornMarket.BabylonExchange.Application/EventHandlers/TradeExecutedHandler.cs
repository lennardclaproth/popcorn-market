using System.Diagnostics;
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

    public TradeExecutedHandler(ITradeRepository tradeRepository, ILogger<TradeExecutedHandler> logger)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
    }

    public async Task Handle(TradeExecuted notification, CancellationToken cancellationToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        var trade = Trade.Create(notification.BuyOrderId,
                                 notification.SellOrderId,
                                 notification.Ticker,
                                 notification.TradePrice,
                                 notification.TradeQuantity,
                                 notification.ExecutedAt);
        await _tradeRepository.AddEntity(trade);
        _logger.LogInformation("Trade between BuyOrderId: {BuyOrderId} and SellOrderId: {SellOrderId} has been persisted in {ElapsedTimeMs} ms",
                               notification.BuyOrderId,
                               notification.SellOrderId,
                               Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);
    }
}
