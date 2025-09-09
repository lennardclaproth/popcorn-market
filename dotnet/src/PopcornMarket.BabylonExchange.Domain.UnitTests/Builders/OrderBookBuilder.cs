using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.UnitTests.Builders;

public class OrderBookBuilder
{
    private Listing _listing = new ListingBuilder().Build();
    private List<Order> _buyOrders = new();
    private List<Order> _sellOrders = new();
    private string _ticker = "BABT:TEST";

    public OrderBookBuilder WithListing(Listing listing)
    {
        _listing = listing;
        return this;
    }

    public OrderBookBuilder WithBuyOrders(IEnumerable<Order> orders)
    {
        _buyOrders = new List<Order>(orders);
        return this;
    }

    public OrderBookBuilder WithSellOrders(IEnumerable<Order> orders)
    {
        _sellOrders = new List<Order>(orders);
        return this;
    }

    public OrderBookBuilder WithTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    public OrderBook Build()
    {
        var orderBookResult = OrderBook.Create(_listing, _ticker);
        var orderBook = orderBookResult.Value!;
        foreach (var order in _buyOrders)
        {
            orderBook.PlaceOrder(order);
        }
        foreach (var order in _sellOrders)
        {
            orderBook.PlaceOrder(order);
        }
        return orderBook;
    }
}
