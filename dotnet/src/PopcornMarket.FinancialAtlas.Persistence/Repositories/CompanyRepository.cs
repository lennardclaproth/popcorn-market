﻿using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;
using PopcornMarket.SharedKernel.Attributes.ServiceLifetime;

namespace PopcornMarket.FinancialAtlas.Persistence.Repositories;

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

    public Task<Company?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task Add(Company entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task Update(string id, Company entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string id)
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
