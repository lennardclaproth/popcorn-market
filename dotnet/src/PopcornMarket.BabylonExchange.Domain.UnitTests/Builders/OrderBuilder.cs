using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Domain.UnitTests.Builders;

public class OrderBuilder
{
    private string _stockSymbol = "BABY:TEST";
    private string _traderId = Guid.NewGuid().ToString();
    private decimal _price = 100m;
    private int _quantity = 10;
    private OrderType _orderType = OrderType.LimitOrder;
    private OrderBook _orderBook = null!;
    private OrderSide _orderSide = OrderSide.Buy;

    public OrderBuilder WithStockSymbol(string symbol)
    {
        _stockSymbol = symbol;
        return this;
    }

    public OrderBuilder WithTraderId(string traderId)
    {
        _traderId = traderId;
        return this;
    }

    public OrderBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public OrderBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public OrderBuilder WithOrderType(OrderType type)
    {
        _orderType = type;
        return this;
    }

    public OrderBuilder WithOrderBook(OrderBook orderBook)
    {
        _orderBook = orderBook;
        return this;
    }

    public OrderBuilder WithOrderSide(OrderSide side)
    {
        _orderSide = side;
        return this;
    }

    public Order Build()
    {
        return Order.Create(_stockSymbol, _traderId, _price, _quantity, _orderType, _orderBook, _orderSide);
    }
}
