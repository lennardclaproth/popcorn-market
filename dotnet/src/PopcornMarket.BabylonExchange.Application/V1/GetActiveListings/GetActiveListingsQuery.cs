using PopcornMarket.BabylonExchange.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.GetActiveListings;
public sealed record GetActiveListingsQuery : IQuery<IEnumerable<ListingDto>>
{
    public string? Filter { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
