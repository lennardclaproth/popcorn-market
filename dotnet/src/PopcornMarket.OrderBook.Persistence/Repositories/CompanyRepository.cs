using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using PopcornMarket.OrderBook.Domain.Abstractions.Repositories;
using PopcornMarket.OrderBook.Domain.Entities;
using PopcornMarket.OrderBook.Persistence.Context;

namespace PopcornMarket.OrderBook.Persistence.Repositories;

internal sealed class CompanyRepository : ICompanyRepository
{
    private readonly OrderBookDbContext _context;

    public CompanyRepository(OrderBookDbContext context)
    {
        _context = context;
    }

    public async Task AddEntity(Company entity)
    {
        await _context.Companies.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task UpdateEntity(Company entity)
    {
        throw new NotImplementedException();
    }

    public Task<Company?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Company>> GetManyBySpecification<TSpec>(TSpec specification) where TSpec : Specification<Company>
    {
        throw new NotImplementedException();
    }

    public Task<Company?> GetBySpecification<TSpec>(TSpec specification) where TSpec : SingleResultSpecification<Company>
    {
        throw new NotImplementedException();
    }

    public async Task<Company?> GetByTicker(string ticker)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Ticker == ticker);
        
        return company;
    }
}
