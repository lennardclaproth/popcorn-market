using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetArticlesBySector;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.GetMacroEconomicArticlesByRegion;
using PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticlesByRegion;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
using PopcornMarket.FinancialTimes.Contracts.V1.Responses;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetMacroEconomicArticlesByRegion;

internal sealed class GetMacroEconomicArticlesByRegion : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/articles/macro/region/{region}", async (
                [AsParameters] GetMacroEconomicArticlesByRegionRequest req,
                ISender sender,
                ILogger<GetArticlesBySectorEndpoint> logger,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await sender.Send(new GetMacroEconomicArticlesByRegionQuery { Region = req.Region, Limit = req.Limit },
                    cancellationToken);

                return result.IsFailure ? result.ToProblemDetails() : Results.Ok(new GetArticlesBySectorResponse
                {
                    Sector   = req.Region,
                    Articles = result.Value ?? new List<ArticleDto>()
                });
            }).AllowAnonymous()
            .AddValidation<GetMacroEconomicArticlesByRegionRequest>();
    }
}
