namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record MarketSnapshot(
    decimal StockPriceUSD,
    decimal MarketCapBillion,
    decimal DividendPerShareUSD,
    decimal DividendYieldPercent,
    DateTime Date);
