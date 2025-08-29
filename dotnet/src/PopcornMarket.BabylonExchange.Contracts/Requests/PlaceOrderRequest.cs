using PopcornMarket.BabylonExchange.Contracts.Enums;

namespace PopcornMarket.BabylonExchange.Contracts.Requests;

public record PlaceOrderRequest
{
    public string TraderId { get; init; } = null!;
    public string Ticker { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public OrderType Type { get; init; }
    public OrderSide Side { get; init; }
}
