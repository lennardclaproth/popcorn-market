using Ardalis.Specification;
using PopcornMarket.OrderBook.Domain.Abstractions.Repositories;
using PopcornMarket.OrderBook.Domain.Entities;

namespace PopcornMarket.OrderBook.Persistence.Repositories;

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

    public Task<IEnumerable<SellOrder>> GetManyBySpecification<TSpec>(TSpec specification) where TSpec : Specification<SellOrder>
    {
        throw new NotImplementedException();
    }

    public Task<SellOrder?> GetBySpecification<TSpec>(TSpec specification) where TSpec : SingleResultSpecification<SellOrder>
    {
        throw new NotImplementedException();
    }
}
