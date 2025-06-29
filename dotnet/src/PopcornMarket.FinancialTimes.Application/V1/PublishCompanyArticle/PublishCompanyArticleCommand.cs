﻿using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishCompanyArticle;

public sealed record PublishCompanyArticleCommand : ICommand<Guid>
{
    public required string TickerSymbol { get; init; }
    public required string Sector { get; init; }
    public required string CompanyName { get; init; }
    public required string Headline { get; init; }
    public required string Content { get; init; }
}
