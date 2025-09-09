using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Domain.Errors;

public static class OrderErrors
{
    public static readonly Error MarketOrderCannotHavePriceSet = 
        Error.Validation("Order.MarketOrderCannotHavePriceSet", "A market order cannot have a price set.");
}
