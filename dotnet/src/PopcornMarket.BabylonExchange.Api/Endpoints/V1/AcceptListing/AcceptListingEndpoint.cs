using MediatR;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.UseCases.AcceptListing;
using PopcornMarket.BabylonExchange.Contracts.Requests;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.AcceptListing;

internal sealed class AcceptListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/listing/{symbol}/accept", async (string symbol, AcceptListingRequest req,ISender _sender, CancellationToken ct) =>
        {
            var command = new AcceptListingCommand { 
                StockSymbol = symbol,
                PublicOfferingPrice = req.PublicOfferingPrice,
                InitialPublicOfferingDate = req.InitialPublicOfferingDate
            };
            var result = await _sender.Send(command, ct);
            
            return result.IsFailure ? result.ToProblemDetails() : Results.Accepted();
        }).WithTransactionName()
            .AllowAnonymous();
    }
}
