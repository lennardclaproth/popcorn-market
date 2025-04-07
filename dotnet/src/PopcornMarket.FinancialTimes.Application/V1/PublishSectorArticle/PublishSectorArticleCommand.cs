using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishSectorArticle;

public sealed record PublishSectorArticleCommand : ICommand
{
    public required string Headline { get; init; }
    public required string Content { get; init; }
    public required string Sector { get; init; }
}
