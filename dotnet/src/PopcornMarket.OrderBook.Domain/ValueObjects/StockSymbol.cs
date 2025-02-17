
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.OrderBook.Domain.ValueObjects;

/// <summary>
/// Ensures stock symbols are valid and consistent (e.g., AAPL, TSLA).
/// </summary>
public class StockSymbol : ValueObject
{
    public string Value { get; }

    public StockSymbol(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Stock symbol cannot be empty.");
        Value = value.ToUpperInvariant();
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
