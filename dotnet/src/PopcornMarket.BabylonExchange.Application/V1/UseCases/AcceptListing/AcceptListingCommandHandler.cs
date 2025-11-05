using MediatR;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.Extensions;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.UseCases.AcceptListing;

internal sealed class AcceptListingCommandHandler : ICommandHandler<AcceptListingCommand>
{
    private readonly IListingRepository _listingRepository;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptListingCommandHandler(IListingRepository listingRepository, IMediator mediator, IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AcceptListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByStockSymbol(request.StockSymbol);
        if (listing == null) return Result.Failure(ListingErrors.ListingWithSymbolNotFound);

        var listingActivationResult = listing.Accept(request.PublicOfferingPrice, request.InitialPublicOfferingDate);

        if (listingActivationResult.IsFailure)
        {
            return listingActivationResult;
        }
        
        await _listingRepository.UpdateEntity(listing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.DispatchDomainEvents(listing, cancellationToken);
        
        return Result.Success();
    }
}
