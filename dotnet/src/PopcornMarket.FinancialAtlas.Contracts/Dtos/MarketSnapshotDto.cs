using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public sealed record MarketSnapshotDto
{
    [JsonPropertyName("stock_price_USD")]
    public decimal StockPriceUSD { get; init; }
    [JsonPropertyName("market_cap_B")]
    public decimal MarketCapBillion { get; init; }
    [JsonPropertyName("volume")]
    public long Volume { get; init; }
    [JsonPropertyName("dividend_per_share_USD")]
    public decimal? DividendPerShareUSD { get; init; }
    [JsonPropertyName("dividend_yield_percent")]
    public decimal? DividendYieldPercent { get; init; }
    [JsonPropertyName("date")]
    public DateTime Date { get; init; }
}
