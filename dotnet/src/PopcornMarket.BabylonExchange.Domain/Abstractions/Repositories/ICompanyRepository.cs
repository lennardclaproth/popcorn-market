using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    public Task<Company?> GetByTicker(string ticker);
}
