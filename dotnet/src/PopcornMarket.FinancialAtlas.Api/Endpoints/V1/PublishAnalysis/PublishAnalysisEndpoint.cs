using MediatR;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.UseCases.PublishAnalysis;
using PopcornMarket.FinancialAtlas.Contracts.Requests;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.PublishAnalysis;

internal sealed class PublishAnalysisEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/company/publish-analysis",
            async (PublishAnalysisRequest request, 
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new PublishAnalysisCommand()
                {
                    Ticker = request.Ticker,
                    Current = request.Current,
                    OneWeek = request.OneWeek,
                    OneMonth = request.OneMonth,
                    ThreeMonths = request.ThreeMonths,
                    TargetPrice = request.TargetPrice
                };

            var result = await sender.Send(command, cancellationToken);
            return result.IsFailure ? result.ToProblemDetails() : Results.Created();
        }).AddValidation<PublishAnalysisRequest>()
        .WithTransactionName().AllowAnonymous();

    }
}
