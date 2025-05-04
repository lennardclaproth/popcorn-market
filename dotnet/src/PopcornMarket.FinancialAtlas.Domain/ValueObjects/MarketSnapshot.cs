namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record MarketSnapshot(
    decimal StockPriceUSD,
    decimal MarketCapBillion,
    long Volume,
    decimal? DividendPerShareUSD,
    decimal? DividendYieldPercent,
    DateTime Date);
