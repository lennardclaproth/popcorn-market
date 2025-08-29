using MediatR;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.ActivateListing;
using PopcornMarket.BabylonExchange.Contracts.Requests;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.ActivateListing;

public class ActivateListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/listing/{id}/activate",
            async (ActivateListingRequest req, ISender sender, CancellationToken ct) =>
            {
                var command = new ActivateListingCommand { Id = req.Id, ReferencePrice = req.ReferencePrice };

                var result = await sender.Send(command, ct);
                return result.IsFailure ? result.ToProblemDetails() : Results.Created();
            }).AllowAnonymous();
    }
}
