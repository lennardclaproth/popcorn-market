using Popcorn.FinancialAtlas.Domain.Entities;

namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface IFinancialStatementRepository : IRepository<FinancialStatement> 
{
    public Task<FinancialStatement?> GetMostRecent(string ticker, CancellationToken cancellationToken);
}
