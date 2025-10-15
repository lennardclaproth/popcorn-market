using MediatR;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.UseCases.ActivateListing;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.ActivateListing;

public class ActivateListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/listing/{symbol}/activate",
            async (string symbol, ISender sender, CancellationToken ct) =>
            {
                var command = new ActivateListingCommand { stockSymbol = symbol };

                var result = await sender.Send(command, ct);
                return result.IsFailure ? result.ToProblemDetails() : Results.Created();
            }).AllowAnonymous();
    }
}
