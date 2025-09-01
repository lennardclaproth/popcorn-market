using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IOrderBookRepository : IRepository<Entities.OrderBook>
{
    public Task<OrderBook?> GetByTicker(string ticker);
    public Task<OrderBook?> GetByTickerIncludingPendingOrders(string ticker);
}
