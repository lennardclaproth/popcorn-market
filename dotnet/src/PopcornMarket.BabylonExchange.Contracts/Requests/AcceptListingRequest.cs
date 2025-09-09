using System.Text.Json.Serialization;

namespace PopcornMarket.BabylonExchange.Contracts.Requests;

public record AcceptListingRequest
{
    [JsonPropertyName("pop")]
    public decimal PublicOfferingPrice { get; init; }
    [JsonPropertyName("ipo_date")]
    public DateTime InitialPublicOfferingDate { get; init; }
}
