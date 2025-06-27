using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishPoliticalArticle;

public sealed record PublishPoliticalArticleCommand : ICommand<Guid>
{
    public required string Headline { get; init; }
    public required string Content { get; init; }
    public required string Region { get; init; }
}
