using MediatR;
using PopcornMarket.FinancialTimes.Api.Abstractions;
using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application.V1.GetArticlesByTicker;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
using PopcornMarket.FinancialTimes.Contracts.V1.Responses;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetArticlesByTicker;

public class GetArticlesByTickerEndpoint : IEndpoint
{
    private readonly ISender _sender;

    public GetArticlesByTickerEndpoint(ISender sender)
    {
        _sender = sender;
    }
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/articles/company/{ticker}", async (
                [AsParameters] GetArticlesByTickerRequest req,
                ISender sender,
                ILogger<GetArticlesByTickerEndpoint> logger,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await sender.Send(new GetArticlesByTickerQuery { Ticker = req.Ticker, Limit = req.Limit },
                    cancellationToken);

                return result.IsFailure ? result.ToProblemDetails() : Results.Ok(new GetArticlesByTickerResponse
                {
                    Ticker   = req.Ticker,
                    Articles = result.Value ?? new List<ArticleDto>()
                });
            }).AllowAnonymous()
            .AddValidation<GetArticlesByTickerRequest>();
    }
}
