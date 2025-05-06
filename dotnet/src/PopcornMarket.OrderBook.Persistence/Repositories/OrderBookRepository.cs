using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using PopcornMarket.OrderBook.Domain.Abstractions.Repositories;
using PopcornMarket.OrderBook.Persistence.Context;

namespace PopcornMarket.OrderBook.Persistence.Repositories;

internal sealed class OrderBookRepository : IOrderBookRepository
{
    private readonly OrderBookDbContext _context;

    public OrderBookRepository(OrderBookDbContext context)
    {
        _context = context;
    }

    public async Task AddEntity(Domain.Entities.OrderBook entity)
    {
        await _context.OrderBooks.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task UpdateEntity(Domain.Entities.OrderBook entity)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Entities.OrderBook?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Domain.Entities.OrderBook>> GetManyBySpecification<TSpec>(TSpec specification) where TSpec : Specification<Domain.Entities.OrderBook>
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Entities.OrderBook?> GetBySpecification<TSpec>(TSpec specification) where TSpec : SingleResultSpecification<Domain.Entities.OrderBook>
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
