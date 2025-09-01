using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Domain.Errors;

public static class ListingErrors
{
    public static readonly Error ListingAlreadyExists =
        Error.Conflict("Listing.AlreadyExists", "The company with the supplied Ticker already exists.");
    
    public static readonly Error ListingNotFound =
        Error.NotFound("Listing.NotFound", "The company with the supplied Ticker was not found.");
    
    public static readonly Error ListingWithIdNotFound = 
        Error.NotFound("Listing.NotFound", "The Listing with the supplied Id was not found.");
    
    public static readonly Error ListingHasNoOrderBook = 
        Error.NotFound("Listing.NoOrderBook", "The listing has no order book.");
    
    public static readonly Error ListingReviewFailedNotNew = 
        Error.Validation("Listing.ReviewFailedNotNew", "The requested listing could not be reviewed while the current state is not new.");
    
    public static readonly Error ListingActivationFailedNotInReview = 
        Error.Validation("Listing.ActivationFailedNotInReview", "The requested listing could not be activated while the current state is not pending.");
}
