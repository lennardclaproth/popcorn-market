using Popcorn.FinancialAtlas.Domain.Entities;

namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByTickerAsync(string ticker);
}
