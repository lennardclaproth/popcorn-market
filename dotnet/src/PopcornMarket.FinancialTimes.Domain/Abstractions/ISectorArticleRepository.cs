using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Domain.Abstractions;

public interface ISectorArticleRepository : IRepository<SectorArticle>
{
    Task<IEnumerable<SectorArticle>> FindBySector(string sector, int limit);
}
