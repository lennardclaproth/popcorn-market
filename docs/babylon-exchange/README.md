# Babylon Exchange

## OrderBook

The orderbook is the entity which holds the trades it keeps the Bids (buy orders) and Asks (sell orders) for a given listing and the history of the orders that have been executed for the given listing.

### L1

In L1 (level 1) we see a 1 level overview of the orderbook, meaning we only see the Best Bid (Highest price someone is willing to buy for) and Best Ask (Lowest price someone is willing to sell for).

### L2

In L2 we see the second level of the orderbook meaning we see every level on the orderbook and the quantity. Beneath an example of an orderbook

| Buy | Sell |
| - | - |
| | 120, 5|
| 110, 5 | |

## Order matching

When order matching it is important to note the following, an order can just sit on the orderbook if the price they want is not matched. This means that if say for example I want to pay at max 100,- for a security and there are currently no securities listed for a 100,- my order will just sit on the orderbook. Now when will this order be fulfilled, either when someone wants to sell and execute the order immediately (market order) and there are no higher orders to match. For selling it means the same, if we want to sell for no lower than 100,- but there is no one willing to buy for a 100,- our order will just sit on the orderbook. However when there is a market order which is for buying with immediate execution and the sell order is the best price at the moment it will get executed. So we can conclude that market orders are always first in first out. If a market order is not executed right away it will just wait until the first order will arrive.

In practice this means that our order execution shall work as follows:

1. We create the order on the orderbook.
2. We add the order to the OrderExecutionQueue (Either buy order or sell order)
3. We try and match the order immediately, if that fails we pass and move on to the next order in line.

the best price is determined as follows.

1. Market orders execute immediately at the best possible price (sell orders for the highest price, buy order for the lowest price).
2. Limit orders execute when the price is matched this happens either for another limit order that matches that price (for sell orders a buy order with a higher price and for buy orders a sell order with a lower price) or immediately for the best price due to a market order match.

For order execution to work properly we need to define a few cases, what happens with market orders, limit orders, partially filling orders, fulfilling orders and cancelling orders.

### Market orders

To match market orders we have the following cases:

1. The order gets fulfilled, meaning the complete quantity of the order gets executed and there is no remainder. When this is done we can publish a trade executed event.
2. The order gets partially filled. When an order gets partially filled we need to publish a trade executed event and the respective events for the orders (both sides). After those events are sent we can have two possible outcomes:
   1. If there are still orders left on the order book to be matched. If this is the case we try and match the order again. Resulting again in a trade executed event and the respective events for the orders (both sides), this order matching loop continues.
   2. If there are no orders left on the order book to be matched it means the orderbook has been depleted. When this occurs we need to cancel the remainder of the order resulting in a order partially cancelled event.
3. The order is not able to be executed due to an depleted orderbook. This case should result into immediate cancellation of the order.

### Limit orders

To match limit orders we have the following cases:

1. The order gets fulfilled, same as the market order. It is able to be fulfilled for the best price immediately.
2. The order gets partially filled.
   1. If there are still orders left that also match the best possible price we keep trying to execute the orders.
   2. If there are no orders left for the best possible price we rest the order on the orderbook.
3. If there are no orders on the order book that match the best price than we can immediately rest the order.

In pseudocode this should look as follows:

``` golang
func (m MatchingEngine) handleOrder(incomingOrder Order){
    success := m.Orderbook.Match(incomingOrder)

    for incomingOrder.RemainginQuantity > 0 && success {
        success := m.Orderbook.Match(incomingOrder)
        publishEvents(m.Orderbook)
    }

    if(success){
        return;
    }

    if(incomingOrder.OrderType == OrderType.LimitOrder){
        m.OrderBook.RestOrder(order)
        return;
    }

    if(incomingOrder.OrderType == OrderType.MarketOrder){
        m.OrderBook.CancelOrder(order)
        publishEvents(m.OrderBook)
        return;
    }
}

func (o OrderBook) Match(incoming Order) bool {
    bestMatch := o.BestMatch(incoming)

    if(bestMatch == nil){
        return false
    }

    tradeQuantity := min(incoming.RemainingQuantity, bestMatch.RemaningQuantity)

    tradePrice := o.GetBestPrice(incoming, bestMatch)

    incoming.TryFulFillOrder(tradePrice, tradeQuantity)
    bestMatch.TryFulFillOrder(tradePrice, tradeQuantity)
    o.CleanupMatchedOrders(incoming, bestMatch)
    return true
}
```

## Listing

To list a company a company needs to request the exchange to be listed there. The exchange usually has a set of requirements which the company needs to fulfill in order to be listed at that exchange.

For this exchange we keep it simple, we have a listing entity. The listing entity should contain the ticker of the company together with the identifier of the exchange. The listing should have a state, for example requested, accepted, active, denied, inactive. The listing should also get an isin which is a unique identifier for the company on the exchange.

The listing process for now should consist of three steps, applying, accepting and activating. When applying the endpoint should receive a ticker and the name of the company

## Resources

1. [Exchange - Investopedia](https://www.investopedia.com/terms/e/exchange.asp)
2. [Listing requirements - Investopedia](https://www.investopedia.com/terms/l/listingrequirements.asp)
3. [Order book - Investopedia](https://www.investopedia.com/terms/o/order-book.asp)
4. [ISIN - Investopedia](https://www.investopedia.com/terms/i/isin.asp)
5. [Order execution 1 - Investopedia](https://www.investopedia.com/terms/e/execution.asp)
6. [Order execution 2 - Investopedia](https://www.investopedia.com/articles/01/022801.asp)
7. [Understand the orderbook like a quant](https://www.youtube.com/watch?v=C24m5WEYWxE)
