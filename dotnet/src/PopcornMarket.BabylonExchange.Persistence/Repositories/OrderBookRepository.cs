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
    }

    public Task UpdateEntity(OrderBook entity)
    {
        _context.OrderBooks.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<OrderBook?> GetById(Guid id)
    {
        return await _context.OrderBooks.FindAsync(id);
    }
    
    public async Task<OrderBook?> GetByStockSymbol(string ticker)
    {
        var orderBook = await _context.OrderBooks
            .FirstOrDefaultAsync(ob => ob.StockSymbol == ticker);
        
        return orderBook;
    }

    public async Task<OrderBook?> GetByStockSymbolIncludingPendingOrdersAsNoTracking(string ticker)
    {
        var buyOrders = await _context.Orders
            .Where(o => o.StockSymbol == ticker 
                        && o.OrderSide == OrderSide.Buy 
                        && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.PartiallyFilled)
                        && o.OrderType != OrderType.MarketOrder)
            .OrderByDescending(o => o.Price)                             // higher price first for limits
            .ThenBy(o => o.PlacedTimestamp)                             // FIFO
            .AsNoTracking()
            .ToListAsync();
        
        var sellOrders = await _context.Orders
            .Where(o => o.StockSymbol == ticker 
                        && o.OrderSide == OrderSide.Sell 
                        && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.PartiallyFilled)
                        && o.OrderType != OrderType.MarketOrder)
            .OrderBy(o => o.Price)                                       // lower price first for limits
            .ThenBy(o => o.PlacedTimestamp)                             // FIFO
            .AsNoTracking()
            .ToListAsync();

        var book = await _context.OrderBooks
            .AsNoTracking()
            .FirstAsync(ob => ob.StockSymbol == ticker);

        foreach (var o in buyOrders) book.PlaceOrder(o);
        foreach (var o in sellOrders) book.PlaceOrder(o);

        return book;
    }
}
