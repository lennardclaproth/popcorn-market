using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IListingRepository : IRepository<Listing>
{
    public Task<Listing?> GetByStockSymbol(string symbol);
    public Task<IReadOnlyCollection<Listing>> GetActiveListings(string? filter, int pageNumber, int pageSize);
}
