using PopcornMarket.FinancialTimes.Domain.Enums;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialTimes.Domain.Entities;

public abstract class Article : Entity
{
    public DateTime PublishDate { get; init; }
    public ArticleType Type { get; init; }
    public string Headline { get; init; } = null!;
    public string Content { get; init; } = null!;

    protected Article(ArticleType type, string headline, string content) : base(Guid.NewGuid())
    {
        PublishDate = DateTime.Now;
        Type = type;
        Headline = headline;
        Content = content;
    }
}
