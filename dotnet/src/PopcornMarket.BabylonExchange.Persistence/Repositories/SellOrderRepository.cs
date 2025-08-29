using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

internal sealed class SellOrderRepository : ISellOrderRepository
{
    public Task AddEntity(SellOrder entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateEntity(SellOrder entity)
    {
        throw new NotImplementedException();
    }

    public Task<SellOrder?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}
