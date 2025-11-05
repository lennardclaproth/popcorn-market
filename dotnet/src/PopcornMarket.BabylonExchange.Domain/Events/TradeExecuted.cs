using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;
public sealed record TradeExecuted : IDomainEvent
{
    public TradeExecuted(Guid buyOrderId,
        Guid sellOrderId,
        decimal tradePrice,
        int tradeQuantity,
        string stockSymbol,
        DateTime executedAt)
    {
        BuyOrderId = buyOrderId;
        SellOrderId = sellOrderId;
        TradePrice = tradePrice;
        TradeQuantity =tradeQuantity;
        StockSymbol = stockSymbol;
        ExecutedAt = executedAt;
    }

    public TradeExecuted(){}

    public Guid BuyOrderId { get; init; }
    public Guid SellOrderId { get; init; }
    public decimal TradePrice { get; init; }
    public int TradeQuantity { get; init; }
    public required string StockSymbol { get; init; } = null!;
    public DateTime ExecutedAt { get; init; }
}
