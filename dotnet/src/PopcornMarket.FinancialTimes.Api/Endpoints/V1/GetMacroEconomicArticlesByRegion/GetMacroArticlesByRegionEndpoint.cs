using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetArticlesBySector;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.GetArticlesBySector;
using PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticlesByRegion;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
using PopcornMarket.FinancialTimes.Contracts.V1.Responses;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetPoliticalArticlesByRegion;

internal sealed class GetPoliticalArticlesByRegion : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/articles/region/{region}", async (
                [AsParameters] GetPoliticalArticlesByRegionRequest req,
                ISender sender,
                ILogger<GetArticlesBySectorEndpoint> logger,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await sender.Send(new GetPoliticalArticlesByRegionQuery { Region = req.Region, Limit = req.Limit },
                    cancellationToken);

                return result.IsFailure ? result.ToProblemDetails() : Results.Ok(new GetArticlesBySectorResponse
                {
                    Sector   = req.Region,
                    Articles = result.Value ?? new List<ArticleDto>()
                });
            }).AllowAnonymous()
            .AddValidation<GetPoliticalArticlesByRegionRequest>();
    }
}
