using Ardalis.GuardClauses;
using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.PublishCompanyArticle;
using PopcornMarket.FinancialTimes.Application.V1.PublishMacroEconomicArticle;
using PopcornMarket.FinancialTimes.Application.V1.PublishPoliticalArticle;
using PopcornMarket.FinancialTimes.Application.V1.PublishSectorArticle;
using PopcornMarket.FinancialTimes.Contracts.V1.Enums;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
using CQRS = PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.PublishArticle;

public class PublishArticleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/articles/publish", async (
            PublishArticleRequest req,
            ISender sender,
            CancellationToken ct
        ) =>
        {
            CQRS.ICommand command = req.ArticleType switch
            {
                ArticleType.Company => new PublishCompanyArticleCommand
                {
                    TickerSymbol = Guard.Against.Null(req.Ticker, nameof(req.Ticker)),
                    Sector = Guard.Against.Null(req.Sector, nameof(req.Sector)),
                    CompanyName = Guard.Against.Null(req.CompanyName, nameof(req.CompanyName)),
                    Headline = req.Headline,
                    Content = req.Content
                },
            
                ArticleType.MacroEconomic => new PublishMacroEconomicArticleCommand()
                {
                    Headline = req.Headline,
                    Content = req.Content,
                    Region = Guard.Against.Null(req.Region, nameof(req.Region)),
                },
            
                ArticleType.Political => new PublishPoliticalArticleCommand()
                {
                    Headline = req.Headline,
                    Content = req.Content,
                    Region = Guard.Against.Null(req.Region, nameof(req.Region)),
                },
            
                ArticleType.Sector => new PublishSectorArticleCommand()
                {
                    Headline = req.Headline,
                    Content = req.Content,
                    Sector = Guard.Against.Null(req.Sector, nameof(req.Sector)),
                },
            
                _ => throw new ArgumentOutOfRangeException(req.ArticleType.ToString())
            };
            
            var result = await sender.Send(command, ct);
            
            return result.IsFailure ? result.ToProblemDetails() : Results.Created();
        }).AllowAnonymous()
        .AddValidation<PublishArticleRequest>()
        .WithName("PublishArticle")
        .WithOpenApi();
    }
}
