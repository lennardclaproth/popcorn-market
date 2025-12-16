using System.Text.Json.Serialization;

namespace PopcornMarket.BabylonExchange.Contracts.Dtos;
public sealed record ListingDto
{
    [JsonPropertyName("symbol")]
    public required string StockSymbol { get; init; }
    [JsonPropertyName("isin")]
    public required string Isin { get; init; }
    [JsonPropertyName("company_name")]
    public required string CompanyName { get; init; }
    [JsonPropertyName("last_price")]
    public required decimal LastPrice { get; set; }
    [JsonPropertyName("price_open")]
    public required decimal PriceOpen { get; init; }
    [JsonPropertyName("price_close")]
    public required decimal PriceClose { get; init; }
    [JsonPropertyName("price_high")]
    public required decimal PriceHigh { get; init; }
    [JsonPropertyName("price_low")]
    public required decimal PriceLow { get; init; }
    [JsonPropertyName("price_change")]
    public required decimal PriceChange { get; init; }
    [JsonPropertyName("price_change_perc")]
    public required decimal PriceChangePercent { get; init; }
    [JsonPropertyName("volume")]
    public required long Volume { get; init; }
    [JsonPropertyName("last_updated")]
    public required DateTimeOffset LastUpdated { get; init; }
}
