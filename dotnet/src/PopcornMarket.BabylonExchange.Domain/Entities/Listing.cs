using System.Xml.Linq;
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
    public OrderBook? OrderBook { get; private set; } = null!;
    private Listing() { }

    /// <summary>
    /// Constructor for tests.
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="name"></param>
    /// <param name="status"></param>
    /// <param name="orderBook"></param>
    private Listing(string ticker, string name, ListingStatus status, OrderBook? orderBook)
    {
        Isin = CreateIsin();
        Ticker = ticker;
        Name = name;
        Status = status;
        OrderBook = orderBook;
    }

    internal static Listing Build(string ticker, string name, ListingStatus status, OrderBook? orderBook)
    {
        return new Listing(ticker, name, status, orderBook);
    }

    /// <summary>
    /// Generates an Exchange unique ticker by combining the ticker with the exchange prefix and generates an Isin.
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="name"></param>
    private Listing(string ticker,
        string name
        ) : base(Guid.NewGuid())
    {
        Isin = CreateIsin();
        Ticker = $"{ExchangeConstants.ExchangeIdentifier}:{ticker}";
        Name = name;
        Status = ListingStatus.Requested;
    }

    private static string CreateIsin()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static Result<Listing> Create(string ticker, string name)
    {
        var company = new Listing(ticker, name);
        return Result<Listing>.Success(company);
    }

    public Result Accept()
    {
        if (Status != ListingStatus.InReview)
        {
            return Result.Failure(ListingErrors.ListingActivationFailedNotInReview);
        }

        var listingAcceptedEvent = new ListingAccepted
        {
            StockSymbol = Ticker
        };
        
        Status = ListingStatus.Accepted;
        RaiseDomainEvent(listingAcceptedEvent);
        
        return Result.Success();
    }

    public Result Review()
    {
        if (Status != ListingStatus.Requested)
        {
            return Result.Failure(ListingErrors.ListingReviewFailedNotNew);
        }
        
        Status = ListingStatus.InReview;
        return Result.Success();
    }
    
    public Result Activate()
    {
        if (OrderBook == null)
        {
            return Result.Failure(ListingErrors.ListingHasNoOrderBook);
        }

        if (OrderBook.CurrentPrice == null)
        {
            return Result.Failure(OrderBookErrors.OrderBookReferenceNotPriceSet);
        }
        
        Status = ListingStatus.Active;
        return Result.Success();
    }
}
