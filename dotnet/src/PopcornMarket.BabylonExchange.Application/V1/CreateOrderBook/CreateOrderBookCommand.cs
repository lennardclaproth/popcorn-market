using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.CreateOrderBook;

public sealed record CreateOrderBookCommand : ICommand
{
    public required string Ticker { get; init; }
}
