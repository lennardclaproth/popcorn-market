using System.Text.Json.Serialization;

namespace PopcornMarket.BabylonExchange.Contracts.Responses;
public record PlaceOrderResponse
{
    [JsonPropertyName("order_id")]
    public string OrderId { get; init; } = null!;
}
