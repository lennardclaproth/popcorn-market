namespace PopcornMarket.BabylonExchange.Domain.Abstractions;
public interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
