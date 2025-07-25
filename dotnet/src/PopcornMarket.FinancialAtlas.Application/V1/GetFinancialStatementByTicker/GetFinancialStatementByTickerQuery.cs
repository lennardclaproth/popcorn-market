using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetFinancialStatementByTicker;

public sealed record GetFinancialStatementByTickerQuery : IQuery<FinancialStatementDto?>
{
    public string Ticker { get; init; } = string.Empty;
}
