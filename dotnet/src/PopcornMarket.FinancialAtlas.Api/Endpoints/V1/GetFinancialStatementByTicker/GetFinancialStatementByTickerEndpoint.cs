using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.GetFinancialStatementByTicker;
using PopcornMarket.FinancialAtlas.Contracts.Requests;
using PopcornMarket.FinancialAtlas.Contracts.Responses;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.GetFinancialStatementByTicker;

internal sealed class GetFinancialStatementByTickerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/financial-statement/{ticker}",
            async ([AsParameters] GetFinancialStatementByTickerRequest req, ISender sender) =>
            {
                var query = new GetFinancialStatementByTickerQuery { Ticker = req.Ticker };
                var result = await sender.Send(query);

                if (!result.IsSuccess)
                {
                    return result.ToProblemDetails();
                }

                Guard.Against.Null(result.Value, nameof(result.Value));

                var response = new GetFinancialStatementByTickerResponse
                {
                    FinancialStatement = result.Value
                };

                return Results.Ok(response);
            }
        );
    }
}
