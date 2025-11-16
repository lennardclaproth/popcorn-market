using MediatR;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.UseCases.PublishFinancialStatement;
using PopcornMarket.FinancialAtlas.Contracts.Requests;
using PopcornMarket.FinancialAtlas.Contracts.Responses;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.PublishFinancialStatement;

internal sealed class PublishFinancialStatementEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/financial-statement/{ticker}",
            async (PublishFinancialStatementRequest request, ISender sender) =>
            {
                var command = new PublishFinancialStatementCommand
                {
                    Ticker = request.Ticker,
                    BalanceSheet = request.BalanceSheet,
                    CashFlowStatement = request.CashFlowStatement,
                    IncomeStatement = request.IncomeStatement,
                    Year = request.Year,
                    Interval = request.Interval,
                    PeriodNumber = request.PeriodNumber,
                };
                
                var result = await sender.Send(command);
                
                return result.IsFailure ? result.ToProblemDetails() : Results.Created("/api/v1/financial-statement/{id}", new PublishFinancialStatementResponse { Id = result.Value});
            }).WithTransactionName().AllowAnonymous();
    }
}
