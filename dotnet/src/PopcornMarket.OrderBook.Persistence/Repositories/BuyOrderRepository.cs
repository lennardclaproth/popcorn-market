using Ardalis.Specification;
using PopcornMarket.OrderBook.Domain.Abstractions.Repositories;
using PopcornMarket.OrderBook.Domain.Entities;

namespace PopcornMarket.OrderBook.Persistence.Repositories;

internal sealed class BuyOrderRepository : IBuyOrderRepository
{
    public Task AddEntity(BuyOrder entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateEntity(BuyOrder entity)
    {
        throw new NotImplementedException();
    }

    public Task<BuyOrder?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BuyOrder>> GetManyBySpecification<TSpec>(TSpec specification) where TSpec : Specification<BuyOrder>
    {
        throw new NotImplementedException();
    }

    public Task<BuyOrder?> GetBySpecification<TSpec>(TSpec specification) where TSpec : SingleResultSpecification<BuyOrder>
    {
        throw new NotImplementedException();
    }
}
