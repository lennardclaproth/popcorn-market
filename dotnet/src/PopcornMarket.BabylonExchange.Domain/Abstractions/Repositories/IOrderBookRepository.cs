using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IOrderBookRepository : IRepository<OrderBook>
{
    public Task<OrderBook?> GetByStockSymbol(string symbol);
    public Task<OrderBook?> GetByStockSymbolIncludingPendingOrders(string ticker);
}
