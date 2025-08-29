using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

internal sealed class CompanyRepository : ICompanyRepository
{
    private readonly BabylonExchangeDbContext _context;

    public CompanyRepository(BabylonExchangeDbContext context)
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
    
    public async Task<Company?> GetByTicker(string ticker)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Ticker == ticker);
        
        return company;
    }
}
