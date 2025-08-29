using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.ActivateListing;

public class ActivateListingCommandHandler : ICommandHandler<ActivateListingCommand>
{
    private readonly IListingRepository _listingRepository;

    public ActivateListingCommandHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Result> Handle(ActivateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetById(request.Id);
        if (listing == null) return Result.Failure(ListingErrors.ListingWithIdNotFound);

        var listingActivationResult = listing.Activate();

        if (listingActivationResult.IsFailure)
        {
            return listingActivationResult;
        }
        
        await _listingRepository.UpdateEntity(listing);
        
        return Result.Success();
    }
}
