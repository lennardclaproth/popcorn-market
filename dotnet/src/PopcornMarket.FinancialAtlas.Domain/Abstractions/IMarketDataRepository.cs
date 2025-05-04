using Popcorn.FinancialAtlas.Domain.Entities;

namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface IMarketDataRepository : IRepository<MarketData>
{
    Task<MarketData?> GetByTicker(string ticker);
}
