using MediatR;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.ApplyForListing;
using PopcornMarket.BabylonExchange.Contracts.Requests;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.ApplyForListing;

public class ApplyForListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/listing/",
            async (ApplyForListingRequest req, ISender sender, CancellationToken ct) =>
            {
                var command = new ApplyForListingCommand
                {
                    Ticker = req.Ticker,
                    Name = req.Name,
                };

                var result = await sender.Send(command, ct);
                return result.IsFailure ? result.ToProblemDetails() : Results.Created();
            }).AllowAnonymous();
    }
}
