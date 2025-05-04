using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;

namespace PopcornMarket.FinancialAtlas.Persistence.Repositories;

internal sealed class MarketDataRepository : IMarketDataRepository
{
    private readonly IMongoCollection<MarketData> _collection;

    public MarketDataRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<MarketData>(DbConstants.MarketDataCollection);
    }
    
    public Task<IEnumerable<MarketData>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<MarketData?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task Add(MarketData entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task Update(Guid id, MarketData entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<MarketData?> GetByTicker(string ticker)
    {
        var filter = Builders<MarketData>.Filter.Eq(c => c.Ticker, ticker);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}
