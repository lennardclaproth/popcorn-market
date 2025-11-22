using System.Data;
using MongoDB.Driver;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.FinancialTimes.Persistence.Constants;
using PopcornMarket.FinancialTimes.Persistence.Context;

namespace PopcornMarket.FinancialTimes.Persistence.Repositories;

internal sealed class CompanyArticleRepository : ICompanyArticleRepository
{
    private readonly IMongoCollection<CompanyArticle> _collection;

    public CompanyArticleRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<CompanyArticle>(DbConstants.CompanyArticleCollection);
    }

    public async Task<IEnumerable<CompanyArticle>> GetAll()
    {
        var cursor = await _collection.FindAsync(Builders<CompanyArticle>.Filter.Empty);
        return cursor.ToEnumerable();
    }

    public async Task<CompanyArticle?> GetById(Guid id)
    {
        var cursor =  await _collection.FindAsync(x => x.Id == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<CompanyArticle>> GetLimit(int limit = 10)
    {
        var cursor = await _collection.Find(Builders<CompanyArticle>.Filter.Empty)
            .Limit(limit)
            .SortByDescending(x => x.PublishDate)
            .ToCursorAsync();
        return await cursor.ToListAsync();
    }

    public async Task Add(CompanyArticle entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task Update(CompanyArticle entity, CancellationToken ct)
    {
        var result = await _collection.ReplaceOneAsync(
            e => e.Id == entity.Id,
            entity,
            new ReplaceOptions { IsUpsert = false },
            ct);
        if (result.MatchedCount == 0)
        {
            throw new DBConcurrencyException($"No companyArticle found with this ID {entity.Id}");
        }
    }

    public async Task Delete(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<CompanyArticle>> FindByTicker(string ticker, int limit = 10)
    {
        var cursor = _collection.Find(x => x.Ticker == ticker)
            .SortByDescending(x => x.PublishDate)
            .Limit(limit);

        return await cursor.ToListAsync();
    }
}
