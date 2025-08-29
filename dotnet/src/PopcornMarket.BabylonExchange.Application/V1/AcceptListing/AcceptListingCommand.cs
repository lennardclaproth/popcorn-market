using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.AcceptListing;

public sealed record AcceptListingCommand : ICommand
{
    public Guid Id { get; init; }
}
