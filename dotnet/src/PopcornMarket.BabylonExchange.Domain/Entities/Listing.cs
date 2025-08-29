using PopcornMarket.BabylonExchange.Domain.Constants;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.BabylonExchange.Domain.ValueObjects;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Domain.Entities;

public sealed class Listing : AggregateRoot
{
    public string Isin { get; set; } = null!;
    public string Ticker { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public ListingStatus Status { get; private set; }
    public Guid? OrderBookId { get; private set; }
    public OrderBook? OrderBook { get; private set; } = null!;

    /// <summary>
    /// Generates an Exchange unique ticker by combining the ticker with the exchange prefix and generates an Isin.
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="name"></param>
    private Listing(string ticker,
        string name
        ) : base(Guid.NewGuid())
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        Isin = new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        Ticker = $"{ExchangeConstants.ExchangeIdentifier}:{ticker}";
        Name = name;
        Status = ListingStatus.Requested;
    }

    public static Result<Listing> Create(string ticker, string name)
    {
        var company = new Listing(ticker, name);
        return Result<Listing>.Success(company);
    }

    public Result Accept()
    {
        if (Status != ListingStatus.Pending)
        {
            return Result.Failure(ListingErrors.ListingActivationFailedListingIsNotPending);
        }

        var listingAcceptedEvent = new ListingAccepted
        {
            StockSymbol = Ticker
        };
        
        Status = ListingStatus.Accepted;
        RaiseDomainEvent(listingAcceptedEvent);
        
        return Result.Success();
    }
    
    public Result Activate()
    {
        if (OrderBookId == null || OrderBookId == Guid.Empty || OrderBook == null)
        {
            return Result.Failure(ListingErrors.ListingHasNoOrderBook);
        }

        if (OrderBook.CurrentPrice == null)
        {
            return Result.Failure(OrderBookErrors.OrderBookNoReferencePriceSet);
        }
        
        Status = ListingStatus.Active;
        return Result.Success();
    }
}
