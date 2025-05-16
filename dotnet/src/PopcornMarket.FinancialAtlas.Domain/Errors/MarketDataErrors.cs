using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Errors;

public static class MarketDataErrors
{
    public static readonly Error MarketDataAlreadyExists =
        Error.Conflict("MarketData.AlreadyExists", "The market data with the supplied ticker already exists.");
    
    public static readonly Error MarketDataNotFound = 
        Error.NotFound("MarketData.NotFound", "The market for the supplied ticker could not be found.");
}
