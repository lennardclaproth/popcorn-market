using PopcornMarket.OrderBook.Domain.Entities;

namespace PopcornMarket.OrderBook.Domain.Abstractions.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    public Task<Company?> GetByTicker(string ticker);
}
