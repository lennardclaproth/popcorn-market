namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record Analysis(
    string Ticker,
    float Current,
    float OneWeek,
    float OneMonth,
    float ThreeMonths,
    decimal TargetPrice,
    DateTime Date
);
