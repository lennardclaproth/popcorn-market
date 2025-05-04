namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(Guid id);
    Task Add(T entity);
    Task Update(Guid id, T entity);
    Task Delete(Guid id);
}
