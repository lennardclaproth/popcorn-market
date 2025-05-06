using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.OrderBook.Application.V1.CreateCompany;

public sealed record CreateCompanyCommand : ICommand
{
    public required string Ticker { get; set; }
    public required string Name { get; set; }
    public required decimal StockPriceUSD { get; set; }
}
