using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.BabylonExchange.Domain.Abstractions;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task AddEntity(TEntity entity);
    Task UpdateEntity(TEntity entity);
    Task<TEntity?> GetById(Guid id);
}
