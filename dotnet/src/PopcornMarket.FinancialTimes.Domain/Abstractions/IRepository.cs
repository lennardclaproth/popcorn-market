namespace PopcornMarket.FinancialTimes.Domain.Abstractions;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(Guid id);
    Task<IEnumerable<T>> GetLimit(int limit);
    Task Add(T entity);
    Task Update(Guid id, T entity);
    Task Delete(Guid id);
}
