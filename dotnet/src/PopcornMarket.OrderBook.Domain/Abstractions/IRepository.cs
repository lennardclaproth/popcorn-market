using Ardalis.Specification;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.OrderBook.Domain.Abstractions;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task AddEntity(TEntity entity);
    Task UpdateEntity(TEntity entity);
    Task<TEntity?> GetById(Guid id); 
    Task<IEnumerable<TEntity>> GetManyBySpecification<TSpec>(TSpec specification) where TSpec : Specification<TEntity>;
    Task<TEntity?> GetBySpecification<TSpec>(TSpec specification) where TSpec : SingleResultSpecification<TEntity>;
}
