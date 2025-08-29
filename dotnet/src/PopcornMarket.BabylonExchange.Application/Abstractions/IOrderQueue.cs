using System.Diagnostics.CodeAnalysis;
using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Application.Abstractions;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public interface IOrderQueue
{
    public Task Enqueue(Order order);
}
