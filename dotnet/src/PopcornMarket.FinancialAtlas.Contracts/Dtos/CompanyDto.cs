using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public sealed record CompanyDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    [JsonPropertyName("ticker")]
    public required string Ticker { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("industry")]
    public required string Industry { get; init; }
    [JsonPropertyName("description")]
    public required string Description { get; init; }
    [JsonPropertyName("headquarters")]
    public required string Headquarters { get; init; }
    [JsonPropertyName("ceo")]
    public required string Ceo { get; init; }
    [JsonPropertyName("founded_year")]
    public required int FoundedYear { get; init; }
    [JsonPropertyName("employees")]
    public required int Employees { get; init; }
    [JsonPropertyName("region")]
    public required string Region { get; init; }
}
