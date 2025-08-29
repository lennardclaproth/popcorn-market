using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IListingRepository : IRepository<Listing>
{
    public Task<Listing?> GetByTicker(string ticker);
}
