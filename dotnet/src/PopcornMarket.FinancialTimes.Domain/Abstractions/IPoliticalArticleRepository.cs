using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Domain.Abstractions;

public interface IPoliticalArticleRepository : IRepository<PoliticalArticle>
{
    public Task<IEnumerable<PoliticalArticle>> GetArticlesByRegion(string region, int limit);
}
