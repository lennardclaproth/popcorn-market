using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;

public record ListingAccepted : IDomainEvent
{
    public required string StockSymbol { get; init; }
}
