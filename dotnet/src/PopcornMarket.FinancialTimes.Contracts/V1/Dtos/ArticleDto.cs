using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PopcornMarket.FinancialTimes.Contracts.V1.Enums;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Dtos;

public record ArticleDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("publish_date")]
    public DateTime PublishDate { get; set; }

    [JsonPropertyName("type")]
    public ArticleType Type { get; set; }

    [Required]
    [JsonPropertyName("headline")]
    public required string Headline { get; set; }

    [Required]
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("ticker")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Ticker { get; set; } 

    [JsonPropertyName("industry")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Industry { get; set; }

    [JsonPropertyName("company_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CompanyName { get; set; }

    [JsonPropertyName("region")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Region { get; set; }

    [JsonPropertyName("sector")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Sector { get; set; }
}
