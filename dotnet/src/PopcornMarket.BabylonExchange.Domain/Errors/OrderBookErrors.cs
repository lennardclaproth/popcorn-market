using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Domain.Errors;

public static class OrderBookErrors
{
    public static readonly Error OrderBookAlreadyExists =
        Error.Conflict("OrderBook.AlreadyExists", "The order book with the supplied stock symbol already exists.");
    
    public static readonly Error OrderBookNotFound =
        Error.NotFound("OrderBook.NotFound", "The order book with the supplied stock symbol was not found.");
    
    public static readonly Error OrderBookReferenceNotPriceSet = 
        Error.Validation("OrderBook.ReferencePriceNotSet", "The order book has no price set.");
    
    public static readonly Error OrderBookReferencePriceSet = 
        Error.Validation("OrderBook.ReferencePriceSet", "The order book has a reference price set.");
}
