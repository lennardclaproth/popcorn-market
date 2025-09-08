using MediatR;
using Microsoft.AspNetCore.Mvc;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.GetActiveListings;
using PopcornMarket.BabylonExchange.Contracts.Requests;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.GetActiveListings;

public class ApplyForListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/listing/",
            async ([FromQuery] string? filter, [FromQuery] int pageNumber, [FromQuery] int pageSize, ISender sender, CancellationToken ct) =>
            {
                var command = new GetActiveListingsQuery
                {
                    Filter = filter,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await sender.Send(command, ct);
                return result.IsFailure ? result.ToProblemDetails() : Results.Created();
            }).AllowAnonymous();
    }
}

