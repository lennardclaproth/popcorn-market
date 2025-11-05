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

    public async Task AddEntity(Order entity)
    {
        await _context.Orders.AddAsync(entity);
    }

    public Task UpdateEntity(Order entity)
    {
        _context.Orders.Update(entity);
        return Task.CompletedTask;
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

    public async Task<IReadOnlyCollection<Order>> GetPendingOrdersByTicker(string ticker,
        OrderSide orderSide,
        decimal? lastPrice,
        DateTimeOffset? lastPlacedTimestamp,
        int count)
    {
        var query = _context.Orders
            .Where(o => o.StockSymbol == ticker
                        && o.OrderSide == orderSide
                        && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.PartiallyFilled)
                        && o.OrderType != OrderType.MarketOrder);

        if(orderSide == OrderSide.Buy)
        {
            query = query
                .OrderByDescending(o => o.Price)
                .ThenBy(o => o.PlacedTimestamp);

            if (lastPrice.HasValue && lastPlacedTimestamp.HasValue)
            {
                query = query.Where(o =>
                    o.Price < lastPrice.Value ||
                    (o.Price == lastPrice.Value && o.PlacedTimestamp > lastPlacedTimestamp.Value));
            }
        }
        else // Sell side
        {
            query = query
                .OrderBy(o => o.Price) // lower price first for limits
                .ThenBy(o => o.PlacedTimestamp); // FIFO

            if (lastPrice.HasValue && lastPlacedTimestamp.HasValue)
            {
                query = query.Where(o =>
                    o.Price > lastPrice.Value ||
                    (o.Price == lastPrice.Value && o.PlacedTimestamp > lastPlacedTimestamp.Value));
            }
        }

        query = query.Take(count);
        var orders = await query.ToListAsync();
        return orders;
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
