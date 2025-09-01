using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

internal sealed class OrderBookRepository : IOrderBookRepository
{
    private readonly BabylonExchangeDbContext _context;

    public OrderBookRepository(BabylonExchangeDbContext context)
    {
        _context = context;
    }

    public async Task AddEntity(OrderBook entity)
    {
        await _context.OrderBooks.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEntity(OrderBook entity)
    {
        _context.OrderBooks.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<OrderBook?> GetById(Guid id)
    {
        return await _context.OrderBooks.FindAsync(id);
    }
    
    public async Task<OrderBook?> GetByTicker(string ticker)
    {
        var orderBook = await _context.OrderBooks
            .FirstOrDefaultAsync(ob => ob.Ticker == ticker);
        
        return orderBook;
    }

    public async Task<OrderBook?> GetByTickerIncludingPendingOrders(string ticker)
    {
        var buyOrders = await _context.Orders
            .Where(o => o.StockSymbol == ticker 
                        && o.OrderSide == OrderSide.Buy 
                        && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.PartiallyFilled))
            .OrderBy(o => o.OrderType == OrderType.MarketOrder ? 0 : 1) // market orders first
            .ThenByDescending(o => o.Price)                             // higher price first for limits
            .ThenBy(o => o.PlacedTimestamp)                             // FIFO
            .ToListAsync();
        
        var sellOrders = await _context.Orders
            .Where(o => o.StockSymbol == ticker 
                        && o.OrderSide == OrderSide.Sell 
                        && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.PartiallyFilled))
            .OrderBy(o => o.OrderType == OrderType.MarketOrder ? 0 : 1) // market orders first
            .ThenBy(o => o.Price)                                       // lower price first for limits
            .ThenBy(o => o.PlacedTimestamp)                             // FIFO
            .ToListAsync();

        var book = await _context.OrderBooks.FirstAsync(ob => ob.Ticker == ticker);

        foreach (var o in buyOrders) book.AddOrder(o);
        foreach (var o in sellOrders) book.AddOrder(o);

        return book;
    }
}
