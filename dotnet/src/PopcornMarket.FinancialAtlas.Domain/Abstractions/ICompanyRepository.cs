using Popcorn.FinancialAtlas.Domain.Entities;

namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByTicker(string ticker);
    Task<IEnumerable<string>> GetTickers();
}
