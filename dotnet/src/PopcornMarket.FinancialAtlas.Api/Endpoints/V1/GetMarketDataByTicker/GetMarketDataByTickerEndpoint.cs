using Ardalis.GuardClauses;
using MediatR;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.GetMarketDataByTicker;
using PopcornMarket.FinancialAtlas.Contracts.Requests;
using PopcornMarket.FinancialAtlas.Contracts.Responses;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.GetMarketDataByTicker;

internal sealed class GetMarketDataByTickerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/market-data/{ticker}/current", async (
            [AsParameters] GetMarketDataByTickerRequest req,
            ISender sender,
            ILogger<GetMarketDataByTickerEndpoint> logger,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetMarketDataByTickerQuery { Ticker = req.Ticker, };

            var result = await sender.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ToProblemDetails();
            }

            Guard.Against.Null(result.Value, nameof(result.Value));

            var response = new GetMarketDataByTickerResponse { MarketSnapshot = result.Value };

            return Results.Ok(response);
        }).AllowAnonymous();
    }
}
