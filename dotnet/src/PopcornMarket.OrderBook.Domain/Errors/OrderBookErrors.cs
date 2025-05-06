using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.OrderBook.Domain.Errors;

public static class OrderBookErrors
{
    public static readonly Error OrderBookAlreadyExists =
        Error.Conflict("OrderBook.AlreadyExists", "The order book with the supplied Ticker already exists.");
    
    public static readonly Error OrderBookNotFound =
        Error.NotFound("OrderBook.NotFound", "The order book with the supplied Ticker was not found.");
}
