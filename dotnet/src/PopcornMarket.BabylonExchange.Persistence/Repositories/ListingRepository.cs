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
        await _context.Listings.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEntity(Listing entity)
    {
        _context.Listings.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Listing?> GetById(Guid id)
    {
        return await _context.Listings.Where(l => l.Id == id)
            .Include(l => l.OrderBook)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Listing?> GetByTicker(string ticker)
    {
        var listing = await _context.Listings
            .Where(l => l.Ticker == ticker)
            .Include(l => l.OrderBook)
            .FirstOrDefaultAsync(c => c.Ticker == ticker);
        
        return listing;
    }
}
