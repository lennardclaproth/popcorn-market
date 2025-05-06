using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.OrderBook.Application.V1.CreateOrderBook;

public sealed record CreateOrderBookCommand : ICommand
{
    public required string Ticker { get; set; }
}
