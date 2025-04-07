using MediatR;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.GetTickers;
using PopcornMarket.FinancialAtlas.Contracts.Responses;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.GetTickers;

internal sealed class GetTickersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/company/tickers", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTickersQuery(), ct);

            return result.IsSuccess ?
                   Results.Ok(new GetTickersResponse
                   {
                       Tickers = result.Value ?? new List<string>()
                   })
                   : result.ToProblemDetails();
        }).AllowAnonymous();
    }
}
