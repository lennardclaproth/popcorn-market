using MediatR;
using Microsoft.AspNetCore.Mvc;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.UseCases.GetActiveListings;
using PopcornMarket.BabylonExchange.Contracts.Responses;

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

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }
                
                if (result.Value == null || !result.Value.Any())
                {
                    return Results.NoContent();
                }

                var response = new GetActiveListingsResponse
                {
                    Listings = result.Value,
                    PageNumber = pageNumber,
                    PageCount = (int)Math.Ceiling((double)(result.Value.Count()) / pageSize),

                    // Should become total count of set. 
                    TotalCount = result.Value.Count()
                };

                return Results.Ok(response);
            }).AllowAnonymous();
    }
}

