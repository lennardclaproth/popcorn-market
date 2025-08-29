using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Domain.Errors;

public static class OrderBookErrors
{
    public static readonly Error OrderBookAlreadyExists =
        Error.Conflict("OrderBook.AlreadyExists", "The order book with the supplied Ticker already exists.");
    
    public static readonly Error OrderBookNotFound =
        Error.NotFound("OrderBook.NotFound", "The order book with the supplied Ticker was not found.");
    
    public static readonly Error OrderBookNoReferencePriceSet = 
        Error.Validation("OrderBook.NoReferencePriceSet", "The order book has no price set.");
}
