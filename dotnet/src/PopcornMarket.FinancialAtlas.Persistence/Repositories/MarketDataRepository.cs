using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;

namespace PopcornMarket.FinancialAtlas.Persistence.Repositories;

internal sealed class MarketDataRepository : IMarketDataRepository
{
    private readonly IMongoCollection<MarketData> _marketDataCollection;
    private readonly IMongoCollection<Analysis> _analysisCollection;
    private readonly IMongoCollection<MarketHistory> _marketHistoryCollection;

    public MarketDataRepository(MongoDbContext context)
    {
        _marketDataCollection = context.GetCollection<MarketData>(DbConstants.MarketDataCollection);
        _analysisCollection = context.GetCollection<Analysis>(DbConstants.AnalysisCollection);
        _marketHistoryCollection = context.GetCollection<MarketHistory>(DbConstants.MarketHistoryCollection);
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
        await _marketDataCollection.InsertOneAsync(entity);
    }

    public Task Update(MarketData entity, CancellationToken ct)
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
        return await _marketDataCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Analysis?> GetLatestAnalysisByTicker(string ticker)
    {
        var filter = Builders<Analysis>.Filter.Eq(c => c.Ticker, ticker);

        return await _analysisCollection
            .Find(filter)
            .SortByDescending(c => c.Date)
            .FirstOrDefaultAsync();
    }

    public Task<IEnumerable<MarketSnapshot>> GetHistoryByTicker(string ticker)
    {
        throw new NotImplementedException();
    }

    public async Task InsertAnalysis(Analysis analysis)
    {
        await _analysisCollection.InsertOneAsync(analysis);
    }

    public async Task InsertHistory(IEnumerable<MarketHistory> historicData, CancellationToken ct)
    {
        try
        {
            await _marketHistoryCollection.InsertManyAsync(historicData, new InsertManyOptions() { IsOrdered = false }, ct);
        }
        catch (MongoBulkWriteException ex)
        {
            var nonDuplicateErrors = ex.WriteErrors
                .Where(e => e.Category != ServerErrorCategory.DuplicateKey)
                .ToList();

            if (nonDuplicateErrors.Count > 0)
                throw;
        }
    }
}
