using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Entities;

public class MarketData : Entity
{
    public string TickerSymbol { get; private set; } = null!;
    public MarketSnapshot Current { get; private set; } = null!;
    public List<MarketSnapshot> History { get; private set; } = new();

    private MarketData() { }

    public MarketData(string tickerSymbol, MarketSnapshot current) : base(Guid.NewGuid())
    {
        TickerSymbol = tickerSymbol;
        Current = current;
    }

    public static Result<MarketData> Create(string tickerSymbol, MarketSnapshot current)
    {
        var marketData = new MarketData(tickerSymbol, current);
        
        return Result<MarketData>.Success(marketData);
    }

    public void UpdateCurrent(MarketSnapshot snapshot)
    {
        Current = snapshot;
    }

    public void UpdateMarketSnapshot(MarketSnapshot snapshot)
    {
        History.Add(Current);
        Current = snapshot;
    }
}
