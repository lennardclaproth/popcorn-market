using MediatR;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.AcceptListing;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.AcceptListing;

internal sealed class AcceptListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/listing/{id}/accept", async (Guid id, ISender _sender, CancellationToken ct) =>
        {
            var command = new AcceptListingCommand { Id = id };
            var result = await _sender.Send(command, ct);
            
            return result.IsFailure ? result.ToProblemDetails() : Results.Accepted();
        }).AllowAnonymous();
    }
}
