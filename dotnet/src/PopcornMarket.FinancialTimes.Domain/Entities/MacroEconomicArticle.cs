using PopcornMarket.FinancialTimes.Domain.Enums;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Domain.Entities;

public class MacroEconomicArticle : Article
{
    private MacroEconomicArticle(string headline, string content, string region) : base(ArticleType.MacroEconomic, headline, content)
    {
        Region = region;
    }

    public string Region { get; init; } = null!;

    public static Result<MacroEconomicArticle> Create(string headline, string content,
        string region)
    {
        var article = new MacroEconomicArticle(headline, content, region);
        return Result<MacroEconomicArticle>.Success(article);
    }
}
