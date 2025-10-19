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
