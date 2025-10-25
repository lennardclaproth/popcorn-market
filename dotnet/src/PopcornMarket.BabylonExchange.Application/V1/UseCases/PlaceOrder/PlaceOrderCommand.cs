using PopcornMarket.BabylonExchange.Contracts.Enums;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.UseCases.PlaceOrder;

public record PlaceOrderCommand : ICommand<string>
{
    public string TraderId { get; init; } = null!;
    public string StockSymbol { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public OrderType Type { get; init; }
    public OrderSide Side { get; init; }
}
