namespace Popcorn.FinancialAtlas.Domain.Abstractions;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(string id);
    Task Add(T entity);
    Task Update(string id, T entity);
    Task Delete(string id);
}
