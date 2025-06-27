using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishSectorArticle;

public sealed record PublishSectorArticleCommand : ICommand<Guid>
{
    public required string Headline { get; init; }
    public required string Content { get; init; }
    public required string Sector { get; init; }
    public required string Region { get; init; }
}
