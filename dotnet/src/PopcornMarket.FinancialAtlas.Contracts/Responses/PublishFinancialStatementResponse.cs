using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Responses;

public record PublishFinancialStatementResponse
{
    [Required]
    [JsonPropertyName("id")]
    public required Guid? Id { get; init; }
}
