using System.Text.Json.Serialization;
using PopcornMarket.FinancialTimes.Contracts.V1.Enums;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record PublishArticleRequest
{
    [JsonPropertyName("type")]
    public ArticleType ArticleType { get; set; }

    [JsonPropertyName("headline")]
    public string Headline { get; set; } = null!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
    
    // Company Article Fields
    [JsonPropertyName("ticker")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Ticker { get; set; }

    [JsonPropertyName("industry")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Industry { get; set; }

    [JsonPropertyName("company_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CompanyName { get; set; }

    // Macro & Political Article Fields
    [JsonPropertyName("region")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Region { get; set; }

    // Sector Article Fields
    [JsonPropertyName("sector")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Sector { get; set; }
}
