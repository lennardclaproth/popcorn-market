using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public class InMemoryOrderQueue : IOrderQueue
{
    private readonly Channel<Order> _channel;

    public InMemoryOrderQueue(Channel<Order> channel)
    {
        _channel = channel;
    }

    public Task<int> GetQueueCount()
    {
        return Task.FromResult(_channel.Reader.Count);
    }

    public Task Enqueue(Order order)
    {
        if (!_channel.Writer.TryWrite(order))
        {
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }
}
