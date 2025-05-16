using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.GetTickers;
using PopcornMarket.FinancialTimes.Contracts.V1.Responses;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetTickers;

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
