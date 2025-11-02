using FluentValidation;
using PopcornMarket.BabylonExchange.Contracts.Requests;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.PlaceOrder;

public class PlaceOrderRequestValidator : AbstractValidator<PlaceOrderRequest>
{
    public PlaceOrderRequestValidator()
    {
        RuleFor(x => x.StockSymbol)
            .NotEmpty()
            .WithMessage("Stock symbol is required.")
            .MaximumLength(10)
            .WithMessage("Stock symbol cannot exceed 10 characters.");

        RuleFor(x => x.TraderId)
            .NotEmpty().WithMessage("Trader ID is required.")
            .MaximumLength(50)
            .WithMessage("Trader ID cannot exceed 50 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid order type.");

        RuleFor(x => x.Side)
            .IsInEnum()
            .WithMessage("Invalid order side.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .When(x => x.Type == Contracts.Enums.OrderType.LimitOrder)
            .WithMessage("Price must be greater than zero for limit orders.");

        RuleFor(x => x.Price)
            .Equal(0)
            .When(x => x.Type == Contracts.Enums.OrderType.MarketOrder)
            .WithMessage("Price must be zero for market orders.");
    }
}
