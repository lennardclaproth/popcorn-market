using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface ITradeRepository : IRepository<Trade>
{
    Task<Trade> GetLastExecutedTrade(string ticker);
}
