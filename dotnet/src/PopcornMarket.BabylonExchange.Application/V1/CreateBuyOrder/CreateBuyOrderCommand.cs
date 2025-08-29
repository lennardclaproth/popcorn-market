using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.CreateBuyOrder;

public record CreateBuyOrderCommand : ICommand
{
    public string TraderId { get; init; } = null!;
    public string Ticker { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}
