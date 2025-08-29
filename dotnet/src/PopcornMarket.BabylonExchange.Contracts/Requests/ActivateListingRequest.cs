using System.Text.Json.Serialization;

namespace PopcornMarket.BabylonExchange.Contracts.Requests;

public record ActivateListingRequest
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    [JsonPropertyName("reference_price")]
    public decimal ReferencePrice { get; init; }
}
