using System.Text.Json.Serialization;
using PopcornMarket.BabylonExchange.Contracts.Enums;

namespace PopcornMarket.BabylonExchange.Contracts.Requests;

public record PlaceOrderRequest
{
    [JsonPropertyName("trader_id")]
    public required string TraderId { get; init; }
    [JsonPropertyName("symbol")]
    public required string StockSymbol { get; init; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }
    [JsonPropertyName("price")]
    public decimal Price { get; init; }
    [JsonPropertyName("order_type")]
    public OrderType Type { get; init; }
    [JsonPropertyName("order_side")]
    public OrderSide Side { get; init; }
}
