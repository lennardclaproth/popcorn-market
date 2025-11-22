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

    public async Task Update(MacroEconomicArticle entity, CancellationToken ct)
    {
        var result = await _collection.ReplaceOneAsync(
            e => e.Id == entity.Id,
            entity,
            new ReplaceOptions { IsUpsert = false },
            ct);

        if (result.MatchedCount == 0)
        {
            throw new DBConcurrencyException($"MacroEconomicArticle with Id {entity.Id} was not found for update.");
        }
    }

    public async Task Delete(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<MacroEconomicArticle>> GetArticlesByRegion(string region, int limit)
    {
        var cursor = await _collection.Find(x => x.Region == region)
            .Limit(limit)
            .SortByDescending(x => x.PublishDate)
            .ToCursorAsync();
        return await cursor.ToListAsync();
    }
}
