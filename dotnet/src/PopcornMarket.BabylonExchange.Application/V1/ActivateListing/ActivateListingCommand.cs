using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.ActivateListing;

public record ActivateListingCommand : ICommand
{
    public required string stockSymbol { get; init; }
}
