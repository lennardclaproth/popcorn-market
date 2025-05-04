using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Entities;

public class MarketData : Entity
{
    public string Ticker { get; private set; } = null!;
    public MarketSnapshot Current { get; private set; } = null!;
    public long SharesOutstanding { get; private set; }
    private readonly List<MarketSnapshot> _history = new();
    public IReadOnlyCollection<MarketSnapshot> History => _history;

    private MarketData() { }

    private MarketData(string ticker, long sharesOutstanding ,MarketSnapshot current, List<MarketSnapshot> history) : base(Guid.NewGuid())
    {
        _history = history;
        SharesOutstanding = sharesOutstanding;
        Ticker = ticker;
        Current = current;
    }

    public static Result<MarketData> Create(string tickerSymbol, long sharesOutstanding ,MarketSnapshot current, List<MarketSnapshot> history)
    {
        var marketData = new MarketData(tickerSymbol, sharesOutstanding, current, history);
        
        return Result<MarketData>.Success(marketData);
    }

    public void UpdateCurrent(MarketSnapshot snapshot)
    {
        Current = snapshot;
    }

    public void UpdateMarketSnapshot(MarketSnapshot snapshot)
    {
        _history.Add(Current);
        Current = snapshot;
    }
}
