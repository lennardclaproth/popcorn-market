using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.UnitTests.Builders;

namespace PopcornMarket.BabylonExchange.Domain.UnitTests.Entities;

public class OrderTests
{
    [Fact]
    public void Create_ShouldCreateValidLimitOrder_WithValidParameters()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var stockSymbol = "BABY:TEST";
        var traderId = Guid.NewGuid().ToString();
        var price = 100m;
        var quantity = 10;

        // Act
        var order = Order.Create(stockSymbol, traderId, price, quantity, OrderType.LimitOrder, orderBook, OrderSide.Buy);

        // Assert
        Assert.NotNull(order);
        Assert.StartsWith("ORD-", order.OrderId);
        Assert.EndsWith("-BABY", order.OrderId);
        Assert.Equal(stockSymbol, order.StockSymbol);
        Assert.Equal(traderId, order.TraderId);
        Assert.Equal(price, order.Price);
        Assert.Equal(quantity, order.Quantity);
        Assert.Equal(quantity, order.RemainingQuantity);
        Assert.Equal(OrderType.LimitOrder, order.OrderType);
        Assert.Equal(OrderSide.Buy, order.OrderSide);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(orderBook.Id, order.OrderBookId);
        Assert.Equal(orderBook, order.OrderBook);
        Assert.True(order.PlacedTimestamp <= DateTime.UtcNow);
        Assert.Null(order.ExecutedTimestamp);
        Assert.Equal(0, order.ExecutionPrice);
        Assert.Null(order.StatusNote);
    }

    [Fact]
    public void Create_ShouldCreateValidMarketOrder_WithZeroPrice()
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var stockSymbol = "BABY:TEST";
        var traderId = Guid.NewGuid().ToString();
        var quantity = 10;

        // Act
        var order = Order.Create(stockSymbol, traderId, 0, quantity, OrderType.MarketOrder, orderBook, OrderSide.Sell);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(0, order.Price);
        Assert.Equal(OrderType.MarketOrder, order.OrderType);
        Assert.Equal(OrderSide.Sell, order.OrderSide);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WithInvalidStockSymbol(string invalidSymbol)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var traderId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Order.Create(invalidSymbol, traderId, 100m, 10, OrderType.LimitOrder, orderBook, OrderSide.Buy));
        
        Assert.Equal("Stock symbol is required.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_ShouldThrowArgumentException_WithInvalidQuantity(int invalidQuantity)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var stockSymbol = "BABY:TEST";
        var traderId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Order.Create(stockSymbol, traderId, 100m, invalidQuantity, OrderType.LimitOrder, orderBook, OrderSide.Buy));
        
        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_ShouldThrowArgumentException_WithInvalidPriceForLimitOrder(decimal invalidPrice)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var stockSymbol = "BABY:TEST";
        var traderId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Order.Create(stockSymbol, traderId, invalidPrice, 10, OrderType.LimitOrder, orderBook, OrderSide.Buy));
        
        Assert.Equal("Price must be greater than zero on a limit order.", exception.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(500.50)]
    public void Create_ShouldThrowArgumentException_WithNonZeroPriceForMarketOrder(decimal invalidPrice)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var stockSymbol = "BABY:TEST";
        var traderId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Order.Create(stockSymbol, traderId, invalidPrice, 10, OrderType.MarketOrder, orderBook, OrderSide.Buy));
        
        Assert.Equal("Price must be zero on a market order.", exception.Message);
    }

    [Fact]
    public void TryFulfillOrder_ShouldFullyFulfillOrder_WhenTradeQuantityEqualsRemainingQuantity()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var tradePrice = 105m;
        var tradeQuantity = 10;

        // Act
        order.TryFulfillOrder(tradePrice, tradeQuantity);

        // Assert
        Assert.Equal(OrderStatus.Fulfilled, order.Status);
        Assert.Equal(0, order.RemainingQuantity);
        Assert.Equal(tradePrice, order.ExecutionPrice);
        Assert.NotNull(order.ExecutedTimestamp);
        Assert.True(order.ExecutedTimestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void TryFulfillOrder_ShouldPartiallyFulfillOrder_WhenTradeQuantityLessThanRemainingQuantity()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var tradePrice = 105m;
        var tradeQuantity = 3;

        // Act
        order.TryFulfillOrder(tradePrice, tradeQuantity);

        // Assert
        Assert.Equal(OrderStatus.PartiallyFilled, order.Status);
        Assert.Equal(7, order.RemainingQuantity);
        Assert.Equal(0, order.ExecutionPrice); // Should not be set for partial fills
        Assert.Null(order.ExecutedTimestamp); // Should not be set for partial fills
    }

    [Fact]
    public void TryFulfillOrder_ShouldThrowInvalidOperationException_WhenTradeQuantityExceedsRemainingQuantity()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var tradePrice = 105m;
        var tradeQuantity = 15; // Exceeds remaining quantity

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            order.TryFulfillOrder(tradePrice, tradeQuantity));
        
        Assert.Equal("Trade quantity exceeds remaining order quantity.", exception.Message);
    }

    [Fact]
    public void FulfillOrder_ShouldSetOrderToFulfilled_WithValidParameters()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var executionPrice = 105m;
        var fulfilledAt = DateTime.UtcNow.AddMinutes(-1);

        // Act
        order.FulfillOrder(executionPrice, fulfilledAt);

        // Assert
        Assert.Equal(OrderStatus.Fulfilled, order.Status);
        Assert.Equal(0, order.RemainingQuantity);
        Assert.Equal(executionPrice, order.ExecutionPrice);
        Assert.Equal(fulfilledAt, order.ExecutedTimestamp);
    }

    [Fact]
    public void FulfillOrder_ShouldThrowInvalidOperationException_WhenOrderAlreadyFulfilled()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        order.FulfillOrder(105m, DateTime.UtcNow);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            order.FulfillOrder(110m, DateTime.UtcNow));
        
        Assert.Equal("Order is already fulfilled.", exception.Message);
    }

    [Fact]
    public void PartiallyFulfillOrder_ShouldSetOrderToPartiallyFilled_WithValidParameters()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var executionPrice = 105m;
        var tradeQuantity = 3;
        var fulfilledAt = DateTime.UtcNow.AddMinutes(-1);

        // Act
        order.PartiallyFulfillOrder(executionPrice, tradeQuantity, fulfilledAt);

        // Assert
        Assert.Equal(OrderStatus.PartiallyFilled, order.Status);
        Assert.Equal(7, order.RemainingQuantity);
        Assert.Equal(executionPrice, order.ExecutionPrice);
        Assert.Equal(fulfilledAt, order.ExecutedTimestamp);
    }

    [Fact]
    public void PartiallyFulfillOrder_ShouldThrowInvalidOperationException_WhenOrderAlreadyFulfilled()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        order.FulfillOrder(105m, DateTime.UtcNow);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            order.PartiallyFulfillOrder(110m, 5, DateTime.UtcNow));
        
        Assert.Equal("Order is already fulfilled.", exception.Message);
    }

    [Fact]
    public void CancelOrder_ShouldSetOrderToCanceled_WithValidParameters()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var reason = "User requested cancellation";
        var cancelledAt = DateTime.UtcNow.AddMinutes(-1);

        // Act
        order.CancelOrder(reason, cancelledAt);

        // Assert
        Assert.Equal(OrderStatus.Canceled, order.Status);
        Assert.Equal(reason, order.StatusNote);
        Assert.Equal(cancelledAt, order.ExecutedTimestamp);
    }

    [Fact]
    public void CancelOrder_ShouldThrowInvalidOperationException_WhenOrderAlreadyFulfilled()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        order.FulfillOrder(105m, DateTime.UtcNow);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            order.CancelOrder("Test cancellation", DateTime.UtcNow));
        
        Assert.Equal("Cannot cancel a fulfilled order.", exception.Message);
    }

    [Fact]
    public void PartiallyCancelOrder_ShouldSetOrderToPartiallyCanceled_WithValidParameters()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var reason = "Partial cancellation requested";
        var newQuantity = 5;
        var cancelledAt = DateTime.UtcNow.AddMinutes(-1);

        // Act
        order.PartiallyCancelOrder(reason, newQuantity, cancelledAt);

        // Assert
        Assert.Equal(OrderStatus.PartiallyCanceled, order.Status);
        Assert.Equal(newQuantity, order.RemainingQuantity);
        Assert.Equal(reason, order.StatusNote);
        Assert.Equal(cancelledAt, order.ExecutedTimestamp);
    }

    [Fact]
    public void PartiallyCancelOrder_ShouldThrowInvalidOperationException_WhenOrderAlreadyFulfilled()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        order.FulfillOrder(105m, DateTime.UtcNow);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            order.PartiallyCancelOrder("Test partial cancellation", 5, DateTime.UtcNow));
        
        Assert.Equal("Cannot cancel a fulfilled order.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PartiallyCancelOrder_ShouldThrowArgumentException_WithInvalidNewQuantity(int invalidQuantity)
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var reason = "Test partial cancellation";
        var cancelledAt = DateTime.UtcNow;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            order.PartiallyCancelOrder(reason, invalidQuantity, cancelledAt));
        
        Assert.Equal("New quantity must be greater than zero and less than the remaining quantity.", exception.Message);
    }

    [Fact]
    public void PartiallyCancelOrder_ShouldThrowArgumentException_WhenNewQuantityExceedsRemainingQuantity()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();
        
        var reason = "Test partial cancellation";
        var newQuantity = 15; // Exceeds remaining quantity
        var cancelledAt = DateTime.UtcNow;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            order.PartiallyCancelOrder(reason, newQuantity, cancelledAt));
        
        Assert.Equal("New quantity must be greater than zero and less than the remaining quantity.", exception.Message);
    }

    [Fact]
    public void OrderId_ShouldHaveCorrectFormat_WhenOrderCreated()
    {
        // Arrange & Act
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .Build();

        // Assert
        Assert.Matches(@"^ORD-\d{14}\d{3}-\d{3}-BABY$", order.OrderId);
    }

    [Fact]
    public void OrderId_ShouldBeUnique_ForMultipleOrders()
    {
        var orderBook = GetDefaultOrderBook();
        // Arrange & Act
        var order1 = new OrderBuilder()
            .WithOrderBook(orderBook)
            .Build();
        var order2 = new OrderBuilder()
            .WithOrderBook(orderBook)
            .Build();
        var order3 = new OrderBuilder()
            .WithOrderBook(orderBook)
            .Build();

        // Assert
        Assert.NotEqual(order1.OrderId, order2.OrderId);
        Assert.NotEqual(order1.OrderId, order3.OrderId);
        Assert.NotEqual(order2.OrderId, order3.OrderId);
    }

    [Fact]
    public void MultipleFulfillmentOperations_ShouldWorkInSequence_ForPartialFills()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithOrderBook(GetDefaultOrderBook())
            .WithPrice(100m)
            .WithQuantity(10)
            .Build();

        // Act - First partial fill
        order.TryFulfillOrder(105m, 3);
        Assert.Equal(OrderStatus.PartiallyFilled, order.Status);
        Assert.Equal(7, order.RemainingQuantity);

        // Act - Second partial fill
        order.TryFulfillOrder(106m, 4);
        Assert.Equal(OrderStatus.PartiallyFilled, order.Status);
        Assert.Equal(3, order.RemainingQuantity);

        // Act - Final fill
        order.TryFulfillOrder(107m, 3);

        // Assert
        Assert.Equal(OrderStatus.Fulfilled, order.Status);
        Assert.Equal(0, order.RemainingQuantity);
        Assert.Equal(107m, order.ExecutionPrice);
        Assert.NotNull(order.ExecutedTimestamp);
    }

    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public void Create_ShouldCreateOrderWithCorrectSide_ForBothSides(OrderSide orderSide)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();

        // Act
        var order = Order.Create("BABY:TEST", Guid.NewGuid().ToString(), 100m, 10, OrderType.LimitOrder, orderBook, orderSide);

        // Assert
        Assert.Equal(orderSide, order.OrderSide);
    }

    [Theory]
    [InlineData(OrderType.LimitOrder)]
    [InlineData(OrderType.MarketOrder)]
    public void Create_ShouldCreateOrderWithCorrectType_ForBothTypes(OrderType orderType)
    {
        // Arrange
        var listing = new ListingBuilder().Build();
        var orderBook = new OrderBookBuilder().WithListing(listing).Build();
        var price = orderType == OrderType.LimitOrder ? 100m : 0m;

        // Act
        var order = Order.Create("BABY:TEST", Guid.NewGuid().ToString(), price, 10, orderType, orderBook, OrderSide.Buy);

        // Assert
        Assert.Equal(orderType, order.OrderType);
    }

    private static OrderBook GetDefaultOrderBook()
    {
        var orderBook = new OrderBookBuilder()
            .WithTicker("BABY:TEST")
            .Build();
        return orderBook;
    }
}
