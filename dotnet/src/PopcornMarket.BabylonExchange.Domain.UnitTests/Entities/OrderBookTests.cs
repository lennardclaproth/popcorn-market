using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.BabylonExchange.Domain.UnitTests.Builders;

namespace PopcornMarket.BabylonExchange.Domain.UnitTests.Entities;

public class OrderBookTests
{
    [Fact]
    public void PlaceOrder_ShouldAddOrderToCorrectSideAndRaiseOrderPlacedEvent()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).Build();

        var initialEventCount = orderBook.DomainEvents.Count;

        // Act
        orderBook.PlaceOrder(buyOrder);
        orderBook.PlaceOrder(sellOrder);

        // Assert
        Assert.Contains(buyOrder, orderBook.BuyOrders);
        Assert.Contains(sellOrder, orderBook.SellOrders);
        Assert.Contains(buyOrder, orderBook.Orders);
        Assert.Contains(sellOrder, orderBook.Orders);
        Assert.Equal(initialEventCount + 2, orderBook.DomainEvents.Count);

        var orderPlacedEvents = orderBook.DomainEvents.OfType<OrderPlaced>().ToList();
        Assert.Equal(2, orderPlacedEvents.Count);
    }

    [Fact]
    public void MatchOrder_ShouldReturnTrue_WhenOrdersCanBeMatched()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(10).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(10).Build();

        orderBook.RestOrder(sellOrder);

        // Act
        var result = orderBook.MatchOrder(buyOrder);

        // Assert
        Assert.True(result);
        Assert.Equal(OrderStatus.Fulfilled, buyOrder.Status);
        Assert.Equal(OrderStatus.Fulfilled, sellOrder.Status);
    }

    [Fact]
    public void MatchOrder_ShouldReturnFalse_WhenNoMatchingOrdersExist()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(10).Build();

        // Act
        var result = orderBook.MatchOrder(buyOrder);

        // Assert
        Assert.False(result);
        Assert.Equal(OrderStatus.Pending, buyOrder.Status);
    }

    [Fact]
    public void MatchOrder_ShouldHandlePartialFill_WhenQuantitiesDontMatch()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(5).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(10).Build();

        orderBook.RestOrder(sellOrder);

        // Act
        var result = orderBook.MatchOrder(buyOrder);

        // Assert
        Assert.True(result);
        Assert.Equal(OrderStatus.Fulfilled, sellOrder.Status);
        Assert.Equal(OrderStatus.PartiallyFilled, buyOrder.Status);
        Assert.Equal(5, buyOrder.RemainingQuantity);
    }

    [Fact]
    public void MatchOrder_ShouldRaiseCorrectEvents_WhenOrdersMatch()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(10).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(10).Build();

        orderBook.RestOrder(sellOrder);
        var initialEventCount = orderBook.DomainEvents.Count;

        // Act
        orderBook.MatchOrder(buyOrder);

        // Assert
        var newEvents = orderBook.DomainEvents.Skip(initialEventCount).ToList();

        var fulfilledEvents = newEvents.OfType<OrderFulfilled>().ToList();
        var tradeEvent = newEvents.OfType<TradeExecuted>().Single();

        Assert.Equal(2, fulfilledEvents.Count);
        Assert.Equal(buyOrder.Id, tradeEvent.BuyOrderId);
        Assert.Equal(sellOrder.Id, tradeEvent.SellOrderId);
        Assert.Equal(100, tradeEvent.TradePrice);
        Assert.Equal(10, tradeEvent.TradeQuantity);
    }

    [Fact]
    public void CancelOrder_ShouldCancelMarketOrder_WhenNoMatches()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var marketOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderType(OrderType.MarketOrder)
            .WithOrderSide(OrderSide.Buy)
            .WithPrice(0)
            .WithQuantity(10)
            .Build();

        // Act
        orderBook.CancelOrder(marketOrder);

        // Assert
        Assert.Equal(OrderStatus.Canceled, marketOrder.Status);
        Assert.DoesNotContain(marketOrder, orderBook.BuyOrders);
        Assert.DoesNotContain(marketOrder, orderBook.Orders);

        var cancelEvent = orderBook.DomainEvents.OfType<OrderCancelled>().Single();
        Assert.Equal(marketOrder.Id, cancelEvent.OrderId);
        Assert.Equal("Not able to match orders, no matching orders.", cancelEvent.Reason);
    }

    [Fact]
    public void CancelOrder_ShouldPartiallyCancel_WhenOrderIsPartiallyFilled()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(5).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(10).Build();

        orderBook.RestOrder(sellOrder);
        orderBook.MatchOrder(buyOrder); // This will partially fill the buy order

        // Act
        orderBook.CancelOrder(buyOrder);

        // Assert
        Assert.Equal(OrderStatus.PartiallyCanceled, buyOrder.Status);
        Assert.DoesNotContain(buyOrder, orderBook.BuyOrders);
        Assert.DoesNotContain(buyOrder, orderBook.Orders);

        var partialCancelEvent = orderBook.DomainEvents.OfType<OrderPartiallyCancelled>().Single();
        Assert.Equal(buyOrder.Id, partialCancelEvent.OrderId);
        Assert.Equal(5, partialCancelEvent.RemainingQuantity);
    }

    [Fact]
    public void RestOrder_ShouldAddOrderToCorrectSideWithoutRaisingEvent()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).Build();

        var initialEventCount = orderBook.DomainEvents.Count;

        // Act
        orderBook.RestOrder(buyOrder);
        orderBook.RestOrder(sellOrder);

        // Assert
        Assert.Contains(buyOrder, orderBook.BuyOrders);
        Assert.Contains(sellOrder, orderBook.SellOrders);
        Assert.Equal(initialEventCount, orderBook.DomainEvents.Count); // No new events should be raised
    }

    [Fact]
    public void EvictOrder_ShouldRemoveOrderFromAllCollections()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).Build();

        orderBook.RestOrder(buyOrder);
        orderBook.RestOrder(sellOrder);

        // Act
        orderBook.EvictOrder(buyOrder);
        orderBook.EvictOrder(sellOrder);

        // Assert
        Assert.DoesNotContain(buyOrder, orderBook.BuyOrders);
        Assert.DoesNotContain(buyOrder, orderBook.Orders);
        Assert.DoesNotContain(sellOrder, orderBook.SellOrders);
        Assert.DoesNotContain(sellOrder, orderBook.Orders);
    }

    [Theory]
    [InlineData(OrderSide.Buy, OrderType.LimitOrder, 100, OrderSide.Sell, OrderType.LimitOrder, 99, true)]  // Buy 100, sell 99 - should match
    [InlineData(OrderSide.Buy, OrderType.LimitOrder, 100, OrderSide.Sell, OrderType.LimitOrder, 101, false)] // Buy 100, sell 101 - should not match
    [InlineData(OrderSide.Sell, OrderType.LimitOrder, 100, OrderSide.Buy, OrderType.LimitOrder, 101, true)]  // Sell 100, buy 101 - should match
    [InlineData(OrderSide.Sell, OrderType.LimitOrder, 100, OrderSide.Buy, OrderType.LimitOrder, 99, false)]  // Sell 100, buy 99 - should not match
    [InlineData(OrderSide.Buy, OrderType.MarketOrder, 0, OrderSide.Sell, OrderType.LimitOrder, 100, true)]   // Market buy vs limit sell - should match
    [InlineData(OrderSide.Sell, OrderType.MarketOrder, 0, OrderSide.Buy, OrderType.LimitOrder, 100, true)]   // Market sell vs limit buy - should match
    public void MatchOrder_ShouldRespectPriceMatchingRules(OrderSide incomingSide, OrderType incomingType, decimal incomingPrice,
        OrderSide restingSide, OrderType restingType, decimal restingPrice, bool shouldMatch)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();

        var restingOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderSide(restingSide)
            .WithOrderType(restingType)
            .WithPrice(restingPrice)
            .WithQuantity(10)
            .Build();

        var incomingOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderSide(incomingSide)
            .WithOrderType(incomingType)
            .WithPrice(incomingPrice)
            .WithQuantity(10)
            .Build();

        orderBook.RestOrder(restingOrder);

        // Act
        var result = orderBook.MatchOrder(incomingOrder);

        // Assert
        Assert.Equal(shouldMatch, result);

        if (shouldMatch)
        {
            Assert.Equal(OrderStatus.Fulfilled, incomingOrder.Status);
            Assert.Equal(OrderStatus.Fulfilled, restingOrder.Status);

            var tradeEvent = orderBook.DomainEvents.OfType<TradeExecuted>().SingleOrDefault();
            Assert.NotNull(tradeEvent);
        }
        else
        {
            Assert.Equal(OrderStatus.Pending, incomingOrder.Status);
            Assert.DoesNotContain(orderBook.DomainEvents, e => e is TradeExecuted);
        }
    }

    [Theory]
    [InlineData(OrderType.MarketOrder, OrderSide.Buy,0, OrderType.LimitOrder, OrderSide.Sell,100,  100)]
    [InlineData(OrderType.MarketOrder, OrderSide.Sell, 0, OrderType.LimitOrder, OrderSide.Buy, 100, 100)]
    [InlineData(OrderType.LimitOrder, OrderSide.Buy, 100, OrderType.LimitOrder, OrderSide.Sell,95, 95)]
    [InlineData(OrderType.LimitOrder, OrderSide.Sell, 90, OrderType.LimitOrder, OrderSide.Buy,100, 100)]
    public void MatchOrder_ShouldCalculateCorrectTradePrice(OrderType incomingType, OrderSide incomingSide, decimal incomingPrice, OrderType restingType, OrderSide restingSide, decimal restingPrice, decimal expectedPrice)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();

        var restingOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderSide(restingSide)
            .WithOrderType(restingType)
            .WithPrice(restingType == OrderType.LimitOrder ? restingPrice : 0)
            .WithQuantity(10)
            .Build();

        var incomingOrder = new OrderBuilder()
            .WithOrderBook(orderBook)
            .WithOrderSide(incomingSide)
            .WithOrderType(incomingType)
            .WithPrice(incomingType == OrderType.LimitOrder ? incomingPrice : 0)
            .WithQuantity(10)
            .Build();

        orderBook.RestOrder(restingOrder);

        // Act
        orderBook.MatchOrder(incomingOrder);

        // Assert
        var tradeEvent = orderBook.DomainEvents.OfType<TradeExecuted>().Single();
        Assert.Equal(expectedPrice, tradeEvent.TradePrice);
    }

    [Fact]
    public void MatchOrder_ShouldCleanUpFulfilledOrders()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(10).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(10).Build();

        orderBook.RestOrder(sellOrder);

        // Act
        orderBook.MatchOrder(buyOrder);

        // Assert
        // Fulfilled orders should be removed from their respective sorted sets
        Assert.DoesNotContain(buyOrder, orderBook.BuyOrders);
        Assert.DoesNotContain(sellOrder, orderBook.SellOrders);
    }

    [Fact]
    public void MatchOrder_ShouldHandleComplexMultiplePartialMatches()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();

        var sellOrder1 = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(3).Build();
        var sellOrder2 = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(4).Build();
        var sellOrder3 = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(2).Build();

        orderBook.RestOrder(sellOrder1);
        orderBook.RestOrder(sellOrder2);
        orderBook.RestOrder(sellOrder3);

        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(20).Build();

        // Act & Assert - Match against each sell order
        var result1 = orderBook.MatchOrder(buyOrder);
        Assert.True(result1);
        Assert.Equal(OrderStatus.PartiallyFilled, buyOrder.Status);
        Assert.Equal(17, buyOrder.RemainingQuantity); // 20 - 3 = 17

        var result2 = orderBook.MatchOrder(buyOrder);
        Assert.True(result2);
        Assert.Equal(OrderStatus.PartiallyFilled, buyOrder.Status);
        Assert.Equal(13, buyOrder.RemainingQuantity); // 17 - 4 = 13

        var result3 = orderBook.MatchOrder(buyOrder);
        Assert.True(result3);
        Assert.Equal(OrderStatus.PartiallyFilled, buyOrder.Status);
        Assert.Equal(11, buyOrder.RemainingQuantity); // 13 - 2 = 11

        // All sell orders should be fulfilled
        Assert.Equal(OrderStatus.Fulfilled, sellOrder1.Status);
        Assert.Equal(OrderStatus.Fulfilled, sellOrder2.Status);
        Assert.Equal(OrderStatus.Fulfilled, sellOrder3.Status);

        // Should have 3 trade events
        var tradeEvents = orderBook.DomainEvents.OfType<TradeExecuted>().ToList();
        Assert.Equal(3, tradeEvents.Count);
    }

    [Fact]
    public void MatchOrder_ShouldRaisePartiallyFilledEvent_WhenOrderIsPartiallyMatched()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var sellOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Sell).WithPrice(100).WithQuantity(5).Build();
        var buyOrder = new OrderBuilder().WithOrderBook(orderBook).WithOrderSide(OrderSide.Buy).WithPrice(100).WithQuantity(10).Build();

        orderBook.RestOrder(sellOrder);

        // Act
        orderBook.MatchOrder(buyOrder);

        // Assert
        var partiallyFilledEvents = orderBook.DomainEvents.OfType<OrderPartiallyFilled>().ToList();
        var fulfilledEvents = orderBook.DomainEvents.OfType<OrderFulfilled>().ToList();

        Assert.Single(partiallyFilledEvents); // Buy order is partially filled
        Assert.Single(fulfilledEvents); // Sell order is fulfilled

        var partialEvent = partiallyFilledEvents.First();
        Assert.Equal(buyOrder.Id, partialEvent.Id);
        Assert.Equal(5, partialEvent.RemainingQuantity);
    }

    [Fact]
    public void Create_ShouldReturnSuccessResult_WithValidParameters()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var stockSymbol = "TEST:STOCK";

        // Act
        var result = OrderBook.Create(listing, stockSymbol);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(stockSymbol, result.Value.StockSymbol);
        Assert.Equal(listing.Id, result.Value.ListingId);
    }

    [Fact]
    public void Create_ShouldThrowArgumentNullException_WithNullStockSymbol()
    {
        // Arrange
        var listing = new ListingBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => OrderBook.Create(listing, null!));
        Assert.Throws<ArgumentNullException>(() => OrderBook.Create(listing, ""));
        Assert.Throws<ArgumentNullException>(() => OrderBook.Create(listing, "   "));
    }
}
