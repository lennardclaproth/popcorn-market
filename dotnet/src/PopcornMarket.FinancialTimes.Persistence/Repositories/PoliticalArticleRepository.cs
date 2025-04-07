using MongoDB.Driver;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.FinancialTimes.Persistence.Constants;
using PopcornMarket.FinancialTimes.Persistence.Context;

namespace PopcornMarket.FinancialTimes.Persistence.Repositories;

internal sealed class PoliticalArticleRepository : IPoliticalArticleRepository
{
    private readonly IMongoCollection<PoliticalArticle> _collection;

    public PoliticalArticleRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<PoliticalArticle>(DbConstants.PoliticalArticleCollection);
    }
    
    public Task<IEnumerable<PoliticalArticle>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<PoliticalArticle?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PoliticalArticle>> GetLimit(int limit)
    {
        var cursor = await _collection.Find(Builders<PoliticalArticle>.Filter.Empty)
            .Limit(limit)
            .SortByDescending(x => x.PublishDate)
            .ToCursorAsync();
        return await cursor.ToListAsync();
    }

    public async Task Add(PoliticalArticle entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task Update(Guid id, PoliticalArticle entity)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
