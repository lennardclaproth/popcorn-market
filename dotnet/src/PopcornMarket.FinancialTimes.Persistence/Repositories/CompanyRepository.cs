using MongoDB.Driver;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.FinancialTimes.Persistence.Constants;
using PopcornMarket.FinancialTimes.Persistence.Context;

namespace PopcornMarket.FinancialTimes.Persistence.Repositories;

public class CompanyRepository : ICompanyRepository
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

    public Task<IEnumerable<Company>> GetLimit(int limit)
    {
        throw new NotImplementedException();
    }

    public async Task Add(Company entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task Update(Guid id, Company entity)
    {
        throw new NotImplementedException();
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
        var filter = Builders<Company>.Filter.Empty;
        var cursor = await _collection.DistinctAsync(x => x.Ticker, filter);
        return cursor.ToEnumerable();
    }
}

