using PopcornMarket.BabylonExchange.Domain.Constants;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Domain.Entities;

public sealed class Listing : AggregateRoot
{
    public string Isin { get; set; } = null!;
    public string StockSymbol { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public ListingStatus Status { get; private set; }
    public OrderBook? OrderBook { get; private set; } = null!;
    public decimal PublicOfferingPrice { get; private set; } 
    public DateTimeOffset? InitialPublicOfferingDate { get; private set; }
    public decimal OpenPrice { get; private set; }
    public decimal HighPrice { get; private set; }
    public decimal LowPrice { get; private set; }
    public decimal ClosePrice { get; private set; }
    public long Volume { get; private set; }
    public DateTimeOffset LastUpdate { get; private set; }
    private Listing() { }

    /// <summary>
    /// Constructor for tests.
    /// </summary>
    /// <param name="stockSymbol"></param>
    /// <param name="name"></param>
    /// <param name="status"></param>
    /// <param name="orderBook"></param>
    private Listing(string stockSymbol, string name, ListingStatus status, OrderBook? orderBook)
    {
        Isin = CreateIsin();
        StockSymbol = stockSymbol;
        Name = name;
        Status = status;
        OrderBook = orderBook;
        LastUpdate = DateTime.UtcNow;
    }

    internal static Listing Build(string stockSymbol, string name, ListingStatus status, OrderBook? orderBook)
    {
        return new Listing(stockSymbol, name, status, orderBook);
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
        StockSymbol = $"{ExchangeConstants.ExchangeIdentifier}:{ticker}";
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

    public Result Accept(decimal publicOfferingPrice, DateTime initialPublicOfferingDate)
    {
        if (Status != ListingStatus.InReview)
        {
            return Result.Failure(ListingErrors.ListingActivationFailedNotInReview);
        }

        // TODO: implement properly at some point, should be a background service that activates the listing on the listing date
        //if (initialPublicOfferingDate < DateTime.UtcNow.Date)
        //{
        //    return Result.Failure(ListingErrors.ListingActivationFailedInvalidIpoDate);
        //}

        if (publicOfferingPrice <= 0)
        {
            return Result.Failure(ListingErrors.ListingActivationFailedInvalidPopPrice);
        }

        var listingAcceptedEvent = new ListingAccepted
        {
            StockSymbol = StockSymbol
        };

        PublicOfferingPrice = publicOfferingPrice;
        InitialPublicOfferingDate = initialPublicOfferingDate;
        Status = ListingStatus.Accepted;
        LastUpdate = DateTime.UtcNow;

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
        LastUpdate = DateTime.UtcNow;
        return Result.Success();
    }
    
    public Result Activate()
    {
        if (Status != ListingStatus.Accepted)
        {
            return Result.Failure(ListingErrors.ListingActivationFailedNotAccepted);
        }

        if (PublicOfferingPrice == 0)
        {
            return Result.Failure(ListingErrors.ListingActivationFailedInvalidPopPrice);
        }

        Status = ListingStatus.Active;
        OpenPrice = PublicOfferingPrice;
        LastUpdate = DateTime.UtcNow;
        return Result.Success();
    }

    public void ApplyTrade(decimal price, int quantity, DateTime executedAt)
    {
        if (OpenPrice == 0) OpenPrice = price;     // first trade of session
        ClosePrice = price;                   // last trade
        HighPrice = Math.Max(HighPrice, price);
        LowPrice = LowPrice == 0 ? price : Math.Min(LowPrice, price);
        Volume += quantity;
        LastUpdate = executedAt;
    }
}
