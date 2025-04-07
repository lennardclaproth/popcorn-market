using MongoDB.Driver;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.FinancialTimes.Persistence.Constants;
using PopcornMarket.FinancialTimes.Persistence.Context;

namespace PopcornMarket.FinancialTimes.Persistence.Repositories;

internal sealed class SectorArticleRepository : ISectorArticleRepository
{
    private readonly IMongoCollection<SectorArticle> _collection;

    public SectorArticleRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<SectorArticle>(DbConstants.SectorArticleCollection);
    }
    
    public Task<IEnumerable<SectorArticle>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<SectorArticle?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SectorArticle>> GetLimit(int limit)
    {
        var cursor = _collection.Find(Builders<SectorArticle>.Filter.Empty)
            .SortByDescending(x => x.PublishDate)
            .Limit(limit);
        
        return await cursor.ToListAsync();
    }
    
    public async Task Add(SectorArticle entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task Update(Guid id, SectorArticle entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SectorArticle>> FindBySector(string sector, int limit)
    {
        var cursor = await _collection.Find(x => x.Sector == sector)
            .Limit(limit)
            .SortByDescending(x => x.PublishDate)
            .ToCursorAsync();
        return await cursor.ToListAsync();
    }
}
