using Ardalis.GuardClauses;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.ApplyForListing;

internal sealed class ApplyForListingCommandHandler : ICommandHandler<ApplyForListingCommand>
{
    
    private readonly IListingRepository _listingRepository;

    public ApplyForListingCommandHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    /// <summary>
    /// Checks if a listing already exists for a given ticker, if the listing does not exist
    /// it creates a new listing.
    /// </summary>
    /// <param name="applyFor"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> Handle(ApplyForListingCommand applyFor, CancellationToken cancellationToken)
    {
        var company = await _listingRepository.GetByTicker(applyFor.Ticker);
        if(company != null) return Result.Failure(ListingErrors.ListingAlreadyExists);
        
        var creationResult = Listing.Create(applyFor.Ticker, applyFor.Name);
        
        if(creationResult.IsFailure) return creationResult;
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value), "The creation result cannot be null.");
        
        await _listingRepository.AddEntity(creationResult.Value);
        
        return Result.Success();
    }
}
