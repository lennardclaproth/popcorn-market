using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.GetArticlesBySector;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
using PopcornMarket.FinancialTimes.Contracts.V1.Responses;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetArticlesBySector;

internal sealed class GetArticlesBySectorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/articles/sector/{sector}", async (
                [AsParameters] GetArticlesBySectorRequest req,
                ISender sender,
                ILogger<GetArticlesBySectorEndpoint> logger,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await sender.Send(new GetArticlesBySectorQuery { Sector = req.Sector, Limit = req.Limit },
                    cancellationToken);

                return result.IsFailure ? result.ToProblemDetails() : Results.Ok(new GetArticlesBySectorResponse
                {
                    Sector   = req.Sector,
                    Articles = result.Value ?? new List<ArticleDto>()
                });
            }).AllowAnonymous()
            .AddValidation<GetArticlesBySectorRequest>();
    }
}
