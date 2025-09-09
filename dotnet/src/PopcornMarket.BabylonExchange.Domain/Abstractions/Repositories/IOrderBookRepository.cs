using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IOrderBookRepository : IRepository<Entities.OrderBook>
{
    public Task<OrderBook?> GetByStockSymbol(string symbol);
    public Task<OrderBook?> GetByStockSymbolIncludingPendingOrdersAsNoTracking(string symbol);
}
