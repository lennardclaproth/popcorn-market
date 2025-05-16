using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.GetCompanyByTicker;
using PopcornMarket.FinancialAtlas.Contracts.Requests;
using PopcornMarket.FinancialAtlas.Contracts.Responses;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.GetCompanyByTicker;

internal sealed class GetCompanyByTickerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/company/{ticker}", async (
            [AsParameters] GetCompanyByTickerRequest req,
            ISender sender,
            ILogger<GetCompanyByTickerEndpoint> logger,
            CancellationToken ct
        ) =>
        {
            var query = new GetCompanyByTickerQuery { Ticker = req.Ticker };
            var result = await sender.Send(query, ct);

            if (!result.IsSuccess)
            {
                return result.ToProblemDetails();
            }

            Guard.Against.Null(result.Value, nameof(result.Value));

            var response = new GetCompanyByTickerResponse { Company = result.Value };

            return Results.Ok(response);
        }).AllowAnonymous();
    }
}
