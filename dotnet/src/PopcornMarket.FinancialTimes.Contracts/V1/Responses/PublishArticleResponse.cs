using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Responses;

public record PublishArticleResponse
{
    [Required]
    [JsonPropertyName("id")]
    public required Guid? Id { get; init; }
}
