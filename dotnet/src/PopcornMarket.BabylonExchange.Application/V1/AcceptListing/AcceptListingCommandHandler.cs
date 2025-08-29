using MediatR;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.Extensions;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.AcceptListing;

internal sealed class AcceptListingCommandHandler : ICommandHandler<AcceptListingCommand>
{
    private readonly IListingRepository _listingRepository;
    private readonly IMediator _mediator;

    public AcceptListingCommandHandler(IListingRepository listingRepository, IMediator mediator)
    {
        _listingRepository = listingRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(AcceptListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetById(request.Id);
        if (listing == null) return Result.Failure(ListingErrors.ListingWithIdNotFound);

        var listingActivationResult = listing.Accept();

        if (listingActivationResult.IsFailure)
        {
            return listingActivationResult;
        }
        
        await _listingRepository.UpdateEntity(listing);
        await _mediator.DispatchDomainEventsAsync(listing, cancellationToken);
        
        return Result.Success();
    }
}
