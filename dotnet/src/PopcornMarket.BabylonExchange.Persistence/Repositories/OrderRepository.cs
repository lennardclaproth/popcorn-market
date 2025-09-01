using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Persistence.Repositories;

internal sealed class OrderRepository : IOrderRepository
{
    
    private readonly BabylonExchangeDbContext _context;

    public OrderRepository(BabylonExchangeDbContext context)
    {
        _context = context;
    }

    public Task AddEntity(Order entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateEntity(Order entity)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> GetById(Guid id)
    {
        return _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public Task<Dictionary<OrderType, List<Order>>> GetPendingBuyOrders(Guid orderBookId)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<OrderType, List<Order>>> GetPendingSellOrders(Guid orderBookId)
    {
        throw new NotImplementedException();
    }

    // public async Task<Dictionary<OrderType, List<Order>>> GetGroupedPendingBuyOrders(Guid orderBookId)
    // {
    //     var pendingBuyOrders = await _context.BuyOrders.Where(bo => bo.OrderBookId == orderBookId)
    //         .OrderBy(bo => bo.PlacedTimestamp)
    //         .GroupBy(bo => bo.OrderType)
    //         .ToDictionaryAsync(group => group.Key, group => group.ToList());
    //
    //     return pendingBuyOrders;
    // }
}
