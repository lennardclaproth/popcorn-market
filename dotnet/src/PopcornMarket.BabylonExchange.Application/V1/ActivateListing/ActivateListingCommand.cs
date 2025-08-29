using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.ActivateListing;

public record ActivateListingCommand : ICommand
{
    public Guid Id { get; init; }
    public Decimal ReferencePrice { get; init; }
}
