using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public class InMemoryOrderQueue : IOrderQueue
{
    private readonly Channel<Order> _channel;

    public InMemoryOrderQueue(Channel<Order> channel)
    {
        _channel = channel;
    }

    public async Task Enqueue(Order order)
    {
        await _channel.Writer.WriteAsync(order);
    }
}
