using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.BabylonExchange.Domain.Exceptions;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.DomainEventHandlers;

public class ListingAcceptedHandler : IDomainEventHandler<ListingAccepted>
{
    private readonly ILogger<ListingAcceptedHandler> _logger;
    private readonly IListingRepository _listingRepository;
    private readonly IOrderBookRepository _orderBookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ListingAcceptedHandler(IOrderBookRepository orderBookRepository, IListingRepository listingRepository, ILogger<ListingAcceptedHandler> logger, IUnitOfWork unitOfWork)
    {
        _orderBookRepository = orderBookRepository;
        _listingRepository = listingRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ListingAccepted notification, CancellationToken cancellationToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        _logger.LogDebug("Handling ListingAccepted event for stock symbol: {StockSymbol}", notification.StockSymbol);
        var listing = await _listingRepository.GetByStockSymbol(notification.StockSymbol);
        Guard.Against.Null(listing, nameof(listing));

        var orderBook = await _orderBookRepository.GetByStockSymbol(notification.StockSymbol);
        if (orderBook != null) throw new EntityAlreadyExistsException<OrderBook>(notification.StockSymbol);

        var creationResult = OrderBook.Create(listing, notification.StockSymbol);

        if (creationResult.IsFailure) throw new EntityCreationException<OrderBook>(creationResult.Error.Description);

        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value));
        await _orderBookRepository.AddEntity(creationResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var elapsedTimeMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
        _logger.LogDebug("Order book created for stock symbol: {StockSymbol} in {ElapsedTimeMs}", notification.StockSymbol, elapsedTimeMs);
    }
}
