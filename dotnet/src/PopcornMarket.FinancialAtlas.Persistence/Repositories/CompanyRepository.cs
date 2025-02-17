using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;
using PopcornMarket.SharedKernel.Attributes.ServiceLifetime;

namespace PopcornMarket.FinancialAtlas.Persistence.Repositories;

[ScopedLifetime]
public class CompanyRepository : ICompanyRepository
{
    private readonly IMongoCollection<Company> _collection;

    public CompanyRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Company>(DbConstants.CompanyCollection);
    }

    public Task<IEnumerable<Company>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Company?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Company entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task UpdateAsync(string id, Company entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Company?> GetByTickerAsync(string ticker)
    {
        var filter = Builders<Company>.Filter.Eq(c => c.Ticker, ticker);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}
