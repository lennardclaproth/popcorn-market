using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;

namespace PopcornMarket.FinancialAtlas.Persistence.Repositories;

internal sealed class FinancialStatementRepository : IFinancialStatementRepository
{
    private readonly IMongoCollection<FinancialStatement> _collection;

    public FinancialStatementRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<FinancialStatement>(DbConstants.FinancialStatementCollection);
    }
    
    public Task<IEnumerable<FinancialStatement>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<FinancialStatement?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task Add(FinancialStatement entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task Update(FinancialStatement entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<FinancialStatement>> GetByTicker(string ticker)
    {
        var filter = Builders<FinancialStatement>.Filter.Eq(c => c.Ticker, ticker);
        var cursor =  await _collection.FindAsync(filter);
        return cursor.ToEnumerable();
    }
    
    public async Task<FinancialStatement?> GetMostRecent(string ticker, CancellationToken cancellationToken)
    {
        var filter = Builders<FinancialStatement>.Filter.Eq(s => s.Ticker, ticker);
        var sort = Builders<FinancialStatement>.Sort.Descending(x => x.ComparablePeriod);

        return await _collection
            .Find(filter)
            .Sort(sort)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
