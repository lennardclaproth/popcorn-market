using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.UnitOfWork;
internal sealed class EfCoreUnitOfWork : IUnitOfWork
{
    private readonly BabylonExchangeDbContext _context;

    public EfCoreUnitOfWork(BabylonExchangeDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
