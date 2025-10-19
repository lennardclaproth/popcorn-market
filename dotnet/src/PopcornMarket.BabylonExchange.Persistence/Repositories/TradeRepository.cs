using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

public class TradeRepository : ITradeRepository
{
    private readonly BabylonExchangeDbContext _dbContext;

    public TradeRepository(BabylonExchangeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddEntity(Trade entity)
    {
        await _dbContext.Trades.AddAsync(entity);
    }

    public Task<Trade?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateEntity(Trade entity)
    {
        throw new NotImplementedException();
    }
}
