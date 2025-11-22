using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.ValueObjects;

namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface IMarketDataRepository : IRepository<MarketData>
{
    Task<MarketData?> GetByTicker(string ticker);
    Task<Analysis?> GetLatestAnalysisByTicker(string ticker);
    Task<IEnumerable<MarketSnapshot>> GetHistoryByTicker(string ticker);
    Task InsertAnalysis(Analysis analysis);
    Task InsertHistory(IEnumerable<MarketHistory> historicData, CancellationToken ct);
}
