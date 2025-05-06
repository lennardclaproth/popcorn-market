using MediatR;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.PublishMarketData;
using PopcornMarket.FinancialAtlas.Contracts.Requests;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.PublishMarketData;

internal sealed class PublishMarketDataEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/market-data/{ticker}", async (PublishMarketDataRequest request, ISender sender) =>
        {
            var command = new PublishMarketDataCommand
            {
                Ticker = request.Ticker,
                Current = request.Current,
                History = request.History,
                SharesOutstanding = request.SharesOutstanding
            };
            var result = await sender.Send(command);
            return result.IsFailure ? result.ToProblemDetails() : Results.Created();
        }).AllowAnonymous();
    }
}
