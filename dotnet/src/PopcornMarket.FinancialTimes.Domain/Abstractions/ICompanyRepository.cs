using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Domain.Abstractions;

public interface ICompanyRepository : IRepository<Company>
{
    public Task<Company?> GetByTicker(string ticker);
    Task<IEnumerable<string>> GetTickers();
}
