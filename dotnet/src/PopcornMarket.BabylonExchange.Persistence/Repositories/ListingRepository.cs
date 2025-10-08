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
    }

    public Task UpdateEntity(Listing entity)
    {
        _context.Listings.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<Listing?> GetById(Guid id)
    {
        return await _context.Listings.Where(l => l.Id == id)
            .Include(l => l.OrderBook)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Listing?> GetByStockSymbol(string ticker)
    {
        var listing = await _context.Listings
            .Where(l => l.StockSymbol == ticker)
            .Include(l => l.OrderBook)
            .FirstOrDefaultAsync(c => c.StockSymbol == ticker);
        
        return listing;
    }

    public async Task<IReadOnlyCollection<Listing>> GetActiveListings(string? filter, int pageNumber, int pageSize)
    {
        var listings = await _context.Listings
            .OrderBy(l => l.StockSymbol)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Where(l => l.Status == Domain.Enums.ListingStatus.Active)
            .Where(l => filter == null || l.StockSymbol.Contains(filter) || l.Name.Contains(filter) || l.Isin.Contains(filter))
            .ToListAsync();

        return listings;
    }
}
