using PopcornMarket.BabylonExchange.Contracts.Dtos;

namespace PopcornMarket.BabylonExchange.Contracts.Responses;
public sealed record GetActiveListingsResponse
{
    public IEnumerable<ListingDto> Listings { get; init; } = null!;
    public int PageCount { get; init; }
    public int PageNumber { get; init; }
    public int TotalCount { get; init; }
}
