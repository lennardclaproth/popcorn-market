using PopcornMarket.FinancialTimes.Domain.Enums;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Domain.Entities;

public class PoliticalArticle : Article
{
    private PoliticalArticle(string headline, string content, string region) : base(ArticleType.Political, headline, content)
    {
        Region = region;
    }

    public string Region { get; init; } = null!;

    public static Result<PoliticalArticle> Create(string headline, string content,
        string region)
    {
        var article = new PoliticalArticle(headline, content, region);
        
        return Result<PoliticalArticle>.Success(article);
    }
}
