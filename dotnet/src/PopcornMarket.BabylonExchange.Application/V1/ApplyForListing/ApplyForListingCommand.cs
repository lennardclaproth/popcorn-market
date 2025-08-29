using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.ApplyForListing;

public sealed record ApplyForListingCommand : ICommand
{
    public required string Ticker { get; set; }
    public required string Name { get; set; }
}
