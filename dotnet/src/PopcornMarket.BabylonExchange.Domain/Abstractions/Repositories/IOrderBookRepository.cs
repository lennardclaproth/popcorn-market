namespace PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;

public interface IOrderBookRepository : IRepository<Entities.OrderBook>
{
    public Task<Entities.OrderBook?> GetByTicker(string ticker);
}
