using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.BabylonExchange.Application.V1.UseCases.AcceptListing;

public sealed record AcceptListingCommand : ICommand
{
    public required string StockSymbol { get; init; }
    public decimal PublicOfferingPrice { get; init; }
    public DateTime InitialPublicOfferingDate { get; init; }
}
