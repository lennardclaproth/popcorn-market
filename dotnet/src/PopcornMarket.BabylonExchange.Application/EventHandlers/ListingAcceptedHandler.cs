using MediatR;
using PopcornMarket.BabylonExchange.Application.V1.CreateOrderBook;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.EventHandlers;

public class ListingAcceptedHandler : IDomainEventHandler<ListingAccepted>
{
    private readonly ISender _sender;

    public ListingAcceptedHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task Handle(ListingAccepted notification, CancellationToken cancellationToken)
    {
        var command = new CreateOrderBookCommand
        {
            Ticker = notification.StockSymbol
        };
        
        await _sender.Send(command, cancellationToken);
    }
}
