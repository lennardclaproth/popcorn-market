using Ardalis.GuardClauses;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Constants;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.ApplyForListing;

internal sealed class ApplyForListingCommandHandler : ICommandHandler<ApplyForListingCommand>
{
    
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyForListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Checks if a listing already exists for a given ticker, if the listing does not exist
    /// it creates a new listing.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> Handle(ApplyForListingCommand request, CancellationToken cancellationToken)
    {
        var company = await _listingRepository.GetByStockSymbol($"{ExchangeConstants.ExchangeIdentifier}:{request.Ticker}");
        if(company != null) return Result.Failure(ListingErrors.ListingAlreadyExists);
        
        var creationResult = Listing.Create(request.Ticker, request.Name);
        
        if(creationResult.IsFailure) return creationResult;
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value), "The creation result cannot be null.");
        var listing = creationResult.Value;
        listing.Review();
        await _listingRepository.AddEntity(creationResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
