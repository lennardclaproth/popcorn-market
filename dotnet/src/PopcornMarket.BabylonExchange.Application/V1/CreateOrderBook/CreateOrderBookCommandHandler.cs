using Ardalis.GuardClauses;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.CreateOrderBook;

internal sealed class CreateOrderBookCommandHandler : ICommandHandler<CreateOrderBookCommand>
{
    private readonly IOrderBookRepository _orderBookRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderBookCommandHandler(IOrderBookRepository orderBookRepository, IListingRepository listingRepository, IUnitOfWork unitOfWork)
    {
        _orderBookRepository = orderBookRepository;
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateOrderBookCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByStockSymbol(request.StockSymbol);
        if (listing == null) return Result.Failure(ListingErrors.ListingNotFound);
        
        var orderBook = await _orderBookRepository.GetByStockSymbol(request.StockSymbol);
        if (orderBook != null) return Result.Failure(OrderBookErrors.OrderBookAlreadyExists);
        
        var creationResult = OrderBook.Create(listing, request.StockSymbol);
        
        if(creationResult.IsFailure) return creationResult;
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value));
        await _orderBookRepository.AddEntity(creationResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
