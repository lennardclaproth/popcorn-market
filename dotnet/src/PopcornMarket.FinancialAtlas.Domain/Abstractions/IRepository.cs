namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(Guid id);
    Task Add(T entity);
    Task Update(T entity, CancellationToken ct);
    Task Delete(Guid id);
}
