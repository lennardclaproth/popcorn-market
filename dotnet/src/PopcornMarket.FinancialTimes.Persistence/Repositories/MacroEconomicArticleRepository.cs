using System.Data;
using MongoDB.Driver;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.FinancialTimes.Persistence.Constants;
using PopcornMarket.FinancialTimes.Persistence.Context;

namespace PopcornMarket.FinancialTimes.Persistence.Repositories;

internal sealed class MacroEconomicArticleRepository : IMacroEconomicArticleRepository
{
    private readonly IMongoCollection<MacroEconomicArticle> _collection;

    public MacroEconomicArticleRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<MacroEconomicArticle>(DbConstants.MacroEconomicArticleCollection);
    }

    public Task<IEnumerable<MacroEconomicArticle>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<MacroEconomicArticle?> GetById(Guid id)
    {
        var cursor =  await _collection.FindAsync(x => x.Id == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MacroEconomicArticle>> GetLimit(int limit)
    {
        var cursor = await _collection.Find(Builders<MacroEconomicArticle>.Filter.Empty)
            .Limit(limit)
            .SortByDescending(x => x.PublishDate)
            .ToCursorAsync();
        return await cursor.ToListAsync();
    }

    public async Task Add(MacroEconomicArticle entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task Update(Guid id, MacroEconomicArticle entity)
    {
        var filter = Builders<MacroEconomicArticle>.Filter.Eq(x => x.Id, id);
        var update = Builders<MacroEconomicArticle>.Update
            .Set(x => x.PublishDate, entity.PublishDate)
            .Set(x => x.Content, entity.Content)
            .Set(x => x.Headline, entity.Headline);

        var cursor = await _collection.FindAsync(filter);
        var entities = await cursor.ToListAsync();
        
        if (entities.Count > 1) throw new DBConcurrencyException($"More than one MacroEconomic article found with this ID {id}");
        if (entities.Count == 0) throw new DBConcurrencyException($"No MactoEconomic article found with this ID {id}");
        
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task Delete(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
