using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;
public sealed class TradeExecuted : IDomainEvent
{
    public TradeExecuted(Guid buyOrderId, Guid sellOrderId, decimal tradePrice, int tradeQuantity, string ticker, DateTime executedAt)
    {
        BuyOrderId = buyOrderId;
        SellOrderId = sellOrderId;
        TradePrice = tradePrice;
        TradeQuantity = tradeQuantity;
        Ticker = ticker;
        ExecutedAt = executedAt;
    }

    public Guid BuyOrderId { get; }
    public Guid SellOrderId { get; }
    public decimal TradePrice { get; }
    public int TradeQuantity { get; }
    public string Ticker { get; }
    public DateTime ExecutedAt { get; }
}
