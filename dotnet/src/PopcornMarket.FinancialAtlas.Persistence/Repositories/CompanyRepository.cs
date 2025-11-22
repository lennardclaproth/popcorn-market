using System.Data;
using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;

namespace PopcornMarket.FinancialAtlas.Persistence.Repositories;

internal sealed class CompanyRepository : ICompanyRepository
{
    private readonly IMongoCollection<Company> _collection;

    public CompanyRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Company>(DbConstants.CompanyCollection);
    }

    public Task<IEnumerable<Company>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Company?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task Add(Company entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task Update(Company entity, CancellationToken ct)
    {
        var result = await _collection.ReplaceOneAsync(
            e => e.Id == entity.Id,
            entity,
            new ReplaceOptions { IsUpsert = false },
            ct);

        if (result.MatchedCount == 0)
        {
            throw new DBConcurrencyException($"Company {entity.Id} no longer exists.");
        }
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Company?> GetByTicker(string ticker)
    {
        var filter = Builders<Company>.Filter.Eq(c => c.Ticker, ticker);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<string>> GetTickers()
    {
        var filter = Builders<Company>.Filter.Eq(c => c.IsListed, true);
        var cursor = await _collection.DistinctAsync(x => x.Ticker, filter);
        return cursor.ToEnumerable();
    }
}
