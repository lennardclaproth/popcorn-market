using MediatR;
using Microsoft.Extensions.Logging;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.OrderBook.Application.V1.CreateCompany;
using PopcornMarket.OrderBook.Application.V1.CreateOrderBook;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.OrderBook.Infrastructure.Messaging.Handlers;

internal sealed class MarketDataPublishedEventHandler : IEventHandler<MarketDataPublishedEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<MarketDataPublishedEventHandler> _logger;

    public MarketDataPublishedEventHandler(ISender sender, ILogger<MarketDataPublishedEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Handles the creation of a company in this microservice based on the marketdata
    /// that is published. Creates a new OrderBook for that company and a new Company.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task HandleAsync(MarketDataPublishedEvent @event, CancellationToken cancellationToken)
    {
        // Creates a new Company based on the event
        var createCompanyCommand = new CreateCompanyCommand
        {
            Ticker = @event.Ticker, Name = @event.Name, StockPriceUSD = @event.StockPriceUSD,
        };
        var companyCreationResult = await _sender.Send(createCompanyCommand, cancellationToken);
        if (companyCreationResult.IsFailure)
        {
            _logger.LogError(
                "Failed to create company from event. ErrorType: {ErrorType}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}",
                companyCreationResult.Error.Type,
                companyCreationResult.Error.Code,
                companyCreationResult.Error.Description);
            return;
        }

        // Creates a new OrderBook based on the event
        var createOrderBook = new CreateOrderBookCommand { Ticker = @event.Ticker };
        var orderBookCreationResult = await _sender.Send(createOrderBook, cancellationToken);

        if (orderBookCreationResult.IsFailure)
        {
            _logger.LogError(
                "Failed to create orderbook from event. ErrorType: {ErrorType}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}",
                orderBookCreationResult.Error.Type,
                orderBookCreationResult.Error.Code,
                orderBookCreationResult.Error.Description);
        }
    }
}
