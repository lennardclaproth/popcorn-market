using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Domain.Abstractions;

public interface ICompanyArticleRepository : IRepository<CompanyArticle>
{
    public Task<IEnumerable<CompanyArticle>> FindByTicker(string ticker, int limit);
}
