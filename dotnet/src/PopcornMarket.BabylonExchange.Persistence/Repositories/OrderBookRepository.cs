using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

internal sealed class OrderBookRepository : IOrderBookRepository
{
    private readonly BabylonExchangeDbContext _context;

    public OrderBookRepository(BabylonExchangeDbContext context)
    {
        _context = context;
    }

    public async Task AddEntity(BabylonExchange.Domain.Entities.OrderBook entity)
    {
        await _context.OrderBooks.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task UpdateEntity(BabylonExchange.Domain.Entities.OrderBook entity)
    {
        throw new NotImplementedException();
    }

    public Task<BabylonExchange.Domain.Entities.OrderBook?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Domain.Entities.OrderBook?> GetByTicker(string ticker)
    {
        var orderBook = await _context.OrderBooks
            .FirstOrDefaultAsync(ob => ob.Ticker == ticker);
        
        return orderBook;
    }
}
