using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

internal sealed class BuyOrderRepository : IBuyOrderRepository
{
    
    private readonly BabylonExchangeDbContext _context;

    public BuyOrderRepository(BabylonExchangeDbContext context)
    {
        _context = context;
    }

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

    public Task<Dictionary<OrderType, List<BuyOrder>>> GetPendingBuyOrders(Guid orderBookId)
    {
        throw new NotImplementedException();
    }

    public async Task<Dictionary<OrderType, List<BuyOrder>>> GetGroupedPendingBuyOrders(Guid orderBookId)
    {
        var pendingBuyOrders = await _context.BuyOrders.Where(bo => bo.OrderBookId == orderBookId)
            .OrderBy(bo => bo.PlacedTimestamp)
            .GroupBy(bo => bo.OrderType)
            .ToDictionaryAsync(group => group.Key, group => group.ToList());

        return pendingBuyOrders;
    }
}
