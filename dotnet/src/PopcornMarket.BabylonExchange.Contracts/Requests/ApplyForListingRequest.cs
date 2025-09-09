using System.Text.Json.Serialization;

namespace PopcornMarket.BabylonExchange.Contracts.Requests;

public record ApplyForListingRequest
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
}
