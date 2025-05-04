using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticles;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
using PopcornMarket.FinancialTimes.Contracts.V1.Responses;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetPoliticalArticles;

internal sealed class GetPoliticalArticlesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/articles/political", async (
            [AsParameters] GetPoliticalArticlesRequest req,
            ISender sender,
            ILogger<GetPoliticalArticlesEndpoint> logger,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await sender.Send(new GetPoliticalArticlesQuery { Limit = req.Limit },
                cancellationToken);

            return result.IsFailure
                ? result.ToProblemDetails()
                : Results.Ok(new GetPoliticalArticlesResponse
                {
                    Articles = result.Value ?? new List<ArticleDto>()
                });
        }).AllowAnonymous();
    }
}
