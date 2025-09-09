using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Domain.UnitTests.Builders;

public class ListingBuilder
{
    private string _ticker = "TEST";
    private string _name = "Test Company";
    private ListingStatus _status = ListingStatus.Active;
    private OrderBook? _orderBook = null!;

    public ListingBuilder WithTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    public ListingBuilder WithOrderBook(OrderBook orderBook)
    {
        _orderBook = orderBook;
        return this;
    }

    public ListingBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ListingBuilder WithStatus(ListingStatus status)
    {
        _status = status;
        return this;
    }

    public Listing Build()
    {
        //var result = new Listing.Build(_ticker, _name, _status, _orderBook);
        var listing = Listing.Build(_ticker, _name, _status, _orderBook);
        return listing;
    }
}
