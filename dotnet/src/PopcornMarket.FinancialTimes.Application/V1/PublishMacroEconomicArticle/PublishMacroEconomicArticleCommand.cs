using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishMacroEconomicArticle;

public sealed record PublishMacroEconomicArticleCommand : ICommand
{
    public required string Headline { get; init; }
    public required string Content { get; init; }
    public required string Region { get; init; }
}
