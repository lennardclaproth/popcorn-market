using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.BabylonExchange.Domain.UnitTests.Builders;

namespace PopcornMarket.BabylonExchange.Domain.UnitTests.Entities;

public class OrderBookTests
{
    [Fact]
    public void PlaceOrder_Successful_OrderAddedToOrderbook()
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var order = new OrderBuilder().WithOrderBook(orderBook).Build();

        orderBook.PlaceOrder(order);

        Assert.Contains(order, orderBook.Orders);
    }

    [Fact]
    public void MatchOrder_Successful_LimitOrdersToBeMatched()
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        Assert.Equal(OrderStatus.Fulfilled, buyOrder.Status);
        Assert.Equal(OrderStatus.Fulfilled, sellOrder.Status);
    }

    [Theory]
    [InlineData(OrderType.LimitOrder, 100, OrderType.LimitOrder, 101)] // Limit buy trade price = 100, limit sell, trade price = 101
    public void MatchOrder_Successful_UnmatchedOrderToRest(OrderType buyType, decimal buyPrice, OrderType sellType, decimal sellPrice)
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook)
            .WithOrderSide(OrderSide.Buy)
            .WithPrice(buyPrice)
            .WithOrderType(buyType)
            .Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook)
            .WithOrderSide(OrderSide.Sell)
            .WithPrice(sellPrice)
            .WithOrderType(sellType)
            .Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        Assert.Contains(buyOrder, orderBook.Orders);
        Assert.Equal(OrderStatus.Pending, buyOrder.Status);
    }

    [Theory]
    [InlineData(100, 10, 100, 5)] // Partial fill: buy 10, sell 5
    [InlineData(100, 5, 100, 10)] // Partial fill: buy 5, sell 10
    [InlineData(100, 10, 100, 10)] // Full fill: both 10
    public void MatchOrder_Successful_PartialFills(
        decimal buyPrice, int buyQty, decimal sellPrice, int sellQty)
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(buyPrice).WithQuantity(buyQty).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(sellPrice).WithQuantity(sellQty).Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        Assert.Equal(buyQty - Math.Min(buyQty, sellQty), buyOrder.RemainingQuantity);
        Assert.Equal(sellQty - Math.Min(buyQty, sellQty), sellOrder.RemainingQuantity);
    }

    [Fact]
    public void MatchOrder_Successful_PrioritizeBestMatch()
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder1 = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(101).Build();
        var buyOrder2 = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).Build();

        orderBook.PlaceOrder(buyOrder2);
        orderBook.PlaceOrder(buyOrder1);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder1);

        Assert.Equal(OrderStatus.Fulfilled, buyOrder1.Status);
        Assert.Equal(OrderStatus.Fulfilled, sellOrder.Status);
        Assert.Equal(OrderStatus.Pending, buyOrder2.Status);
    }

    [Fact]
    public void PlaceOrder_NegativeQuantity_ThrowsArgumentException()
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();

        Assert.Throws<ArgumentException>(() =>
            new OrderBuilder().WithOrderBook(orderBook).WithQuantity(-5).Build());
    }

    [Theory]
    [InlineData(OrderType.MarketOrder, 0, OrderType.LimitOrder, 100, 10, 10, 100)] // Market buy, limit sell, trade price = 100
    [InlineData(OrderType.LimitOrder, 100, OrderType.MarketOrder, 0, 10, 10, 100)] // Limit buy, market sell, trade price = 100
    [InlineData(OrderType.LimitOrder, 100, OrderType.LimitOrder, 100, 10, 10, 100)]// Limit buy, limit sell, trade price = 100
    public void PlaceOrder_Successful_OrderPlacedEventAndTradeExecutedEventRaised(
        OrderType buyType, decimal buyPrice, OrderType sellType, decimal sellPrice,
        int buyQty, int sellQty, decimal expectedTradePrice)
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderSide(OrderSide.Buy)
            .WithOrderType(buyType)
            .WithPrice(buyPrice)
            .WithQuantity(buyQty)
            .Build();
        var sellOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderSide(OrderSide.Sell)
            .WithOrderType(sellType)
            .WithPrice(sellPrice)
            .WithQuantity(sellQty)
            .Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        // Check OrderPlaced event for both orders
        var placedEvents = orderBook.DomainEvents.OfType<OrderPlaced>().ToList();
        Assert.Contains(placedEvents, e => e.OrderId == buyOrder.Id);
        Assert.Contains(placedEvents, e => e.OrderId == sellOrder.Id);

        // Check TradeExecuted event
        var tradeExecutedEvent = orderBook.DomainEvents.OfType<TradeExecuted>().SingleOrDefault();
        Assert.NotNull(tradeExecutedEvent);
        Assert.Equal(buyOrder.Id, tradeExecutedEvent.BuyOrderId);
        Assert.Equal(sellOrder.Id, tradeExecutedEvent.SellOrderId);
        Assert.Equal(expectedTradePrice, tradeExecutedEvent.TradePrice);
        Assert.Equal(Math.Min(buyQty, sellQty), tradeExecutedEvent.TradeQuantity);
    }



    [Fact]
    public void MatchOrder_Successful_TradeExecutedEventRaisedWithCorrectInputs()
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        var tradeExecutedEvent = orderBook.DomainEvents.OfType<TradeExecuted>().SingleOrDefault();
        Assert.NotNull(tradeExecutedEvent);
        Assert.Equal(buyOrder.Id, tradeExecutedEvent.BuyOrderId);
        Assert.Equal(sellOrder.Id, tradeExecutedEvent.SellOrderId);
        Assert.Equal(100, tradeExecutedEvent.TradePrice);
        Assert.Equal(buyOrder.Quantity, tradeExecutedEvent.TradeQuantity);
    }

    [Fact]
    public void MatchOrder_Successful_OrderFulfilledEventsRaised()
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        var fulfilledEvents = orderBook.DomainEvents.OfType<OrderFulfilled>().ToList();
        Assert.Equal(2, fulfilledEvents.Count);
        Assert.Contains(fulfilledEvents, e => e.Id == buyOrder.Id);
        Assert.Contains(fulfilledEvents, e => e.Id == sellOrder.Id);
    }

    [Theory]
    [InlineData(10, 5, 5, true)]  // Buy order partially filled
    [InlineData(5, 10, 5, false)] // Sell order partially filled
    public void MatchOrder_ShouldRaiseCorrectOrderPartiallyFilledEvent(
        int buyQty, int sellQty, int expectedRemaining, bool isBuyPartial)
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(buyQty).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(sellQty).Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        var partialEvents = orderBook.DomainEvents.OfType<OrderPartiallyFilled>().ToList();
        Assert.Single(partialEvents);

        if (isBuyPartial)
        {
            Assert.Equal(buyOrder.Id, partialEvents[0].Id);
            Assert.Equal(expectedRemaining, partialEvents[0].RemainingQuantity);
        }
        else
        {
            Assert.Equal(sellOrder.Id, partialEvents[0].Id);
            Assert.Equal(expectedRemaining, partialEvents[0].RemainingQuantity);
        }
    }

    [Theory]
    [InlineData(10, 5, true, 5)]  // Buy order partially filled, 10 - 5 = 5 remaining
    [InlineData(5, 10, false, 5)] // Sell order partially filled, 10 - 5 = 5 remaining
    public void MatchOrder_Successful_OrderPartiallyFilledEventRaised(
        int buyQty, int sellQty, bool isBuyPartial, int expectedRemaining)
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(buyQty).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(sellQty).Build();

        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        orderBook.MatchOrder(buyOrder);

        var partialEvents = orderBook.DomainEvents.OfType<OrderPartiallyFilled>().ToList();
        Assert.Single(partialEvents);

        if (isBuyPartial)
        {
            Assert.Equal(buyOrder.Id, partialEvents[0].Id);
            Assert.Equal(expectedRemaining, partialEvents[0].RemainingQuantity);
        }
        else
        {
            Assert.Equal(sellOrder.Id, partialEvents[0].Id);
            Assert.Equal(expectedRemaining, partialEvents[0].RemainingQuantity);
        }
    }

    [Theory]
    [InlineData(OrderType.MarketOrder, OrderSide.Buy, 10, OrderType.LimitOrder, OrderSide.Sell, 100, 0, true, typeof(OrderCancelled))] // No sell orders, market buy should cancel
    [InlineData(OrderType.MarketOrder, OrderSide.Buy, 10, OrderType.LimitOrder, OrderSide.Sell, 100, 5, true, typeof(OrderPartiallyCancelled))] // Partial fill, market buy should partially cancel
    [InlineData(OrderType.LimitOrder, OrderSide.Buy, 90, OrderType.LimitOrder, OrderSide.Sell, 100, 10, false, null)] // Limit order price not matching, should rest
    public void MatchOrder_EdgeCases_CancellationEvents(
        OrderType buyType, OrderSide buySide, int buyQty,
        OrderType sellType, OrderSide sellSide, decimal sellPrice, int sellQty,
        bool expectCancel, Type? expectedEventType)
    {
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();

        var buyOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderSide(buySide)
            .WithOrderType(buyType)
            .WithPrice(buyType == OrderType.LimitOrder ? sellPrice : 0)
            .WithQuantity(buyQty)
            .Build();

        if (sellQty > 0 || sellType == OrderType.MarketOrder)
        {
            var sellOrder = new OrderBuilder()
                .WithOrderBook(orderBook)
                .WithOrderSide(sellSide)
                .WithOrderType(sellType)
                .WithPrice(sellPrice)
                .WithQuantity(sellQty)
                .Build();
            orderBook.PlaceOrder(sellOrder);
        }

        orderBook.PlaceOrder(buyOrder);
        orderBook.MatchOrder(buyOrder);

        if (expectCancel && expectedEventType != null)
        {
            var cancelEvent = orderBook.DomainEvents.FirstOrDefault(e => e.GetType() == expectedEventType);
            Assert.NotNull(cancelEvent);
        }
        else if (expectedEventType == null)
        {
            // Should not cancel, but may rest
            Assert.DoesNotContain(orderBook.DomainEvents, e => e is OrderCancelled || e is OrderPartiallyCancelled);
        }
    }
}
