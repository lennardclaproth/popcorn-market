using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

internal sealed class ListingRepository : IListingRepository
{
    private readonly BabylonExchangeDbContext _context;

    public ListingRepository(BabylonExchangeDbContext context)
    {
        _context = context;
    }

    public async Task AddEntity(Listing entity)
    {
        await _context.Companies.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task UpdateEntity(Listing entity)
    {
        throw new NotImplementedException();
    }

    public Task<Listing?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Listing?> GetByTicker(string ticker)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Ticker == ticker);
        
        return company;
    }
}
