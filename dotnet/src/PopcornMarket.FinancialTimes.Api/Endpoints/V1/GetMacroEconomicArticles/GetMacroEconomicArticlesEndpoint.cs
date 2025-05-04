using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.GetMacroEconomicArticles;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
using PopcornMarket.FinancialTimes.Contracts.V1.Responses;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetMacroEconomicArticles;

internal sealed class GetMacroEconomicArticlesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/articles/macro", async (
            [AsParameters] GetMacroEconomicArticlesRequest req,
            ISender sender,
            ILogger<GetMacroEconomicArticlesEndpoint> logger,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await sender.Send(new GetMacroEconomicArticlesQuery { Limit = req.Limit },
                cancellationToken);

            return result.IsFailure
                ? result.ToProblemDetails()
                : Results.Ok(new GetMacroEconomicArticlesResponse
                {
                    Articles = result.Value ?? new List<ArticleDto>()
                });
        }).AllowAnonymous();
    }
}
