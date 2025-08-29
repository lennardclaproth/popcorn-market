# PopcornMarket.OrderBook
An orderbook is a real-time list of buy and sell orders for a specific
stock organized by its **price level**. It matches buyers and
sellers in the market.

Some core concepts of an orderbook are as follows:
1. **Order Types**
   - Limit Order:
   - Market Order
   - Stop Orders
2. **Sides of the Book**
   - Bid
   - Ask
3. **Matching Engine**
   - The component that **matches** compatible buy and sell order.
   - Follows rules like price-time priority (best price first, then oldest order)

