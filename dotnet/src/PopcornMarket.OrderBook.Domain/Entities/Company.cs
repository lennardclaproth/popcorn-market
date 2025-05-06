using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.OrderBook.Domain.Entities;

public sealed class Company : Entity
{
    public string Ticker { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal StockPriceUSD { get; private set; }

    private Company(string ticker,
        string name,
        decimal stockPriceUSD
        ) : base(Guid.NewGuid())
    {
        Ticker = ticker;
        Name = name;
        StockPriceUSD = stockPriceUSD;
    }

    public static Result<Company> Create(string ticker, string name, decimal stockPriceUSD)
    {
        var company = new Company(ticker, name, stockPriceUSD);
        return Result<Company>.Success(company);
    }
}
