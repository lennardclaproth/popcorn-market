namespace PopcornMarket.FinancialAtlas.Contracts.Requests;

public record GetFinancialStatementByTickerRequest()
{
    public string Ticker { get; init; } = null!;
}
