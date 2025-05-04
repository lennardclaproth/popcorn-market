using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Errors;

public static class MarketDataErrors
{
    public static readonly Error MarketDataAlreadyExists =
        Error.Conflict("MarketData.AlreadyExists", "The market data with the supplied ticker already exists.");
}
