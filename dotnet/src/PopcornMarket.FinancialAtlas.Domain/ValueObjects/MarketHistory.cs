namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record MarketHistory(
    string Ticker,
    decimal StockPriceUSD,
    decimal MarketCapBillion,
    long Volume,
    decimal? DividendPerShareUSD,
    decimal? DividendYieldPercent,
    DateOnly Date);
