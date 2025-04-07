using PopcornMarket.FinancialTimes.Domain.Enums;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Domain.Entities;

public class SectorArticle : Article
{
    private SectorArticle(string headline, string content, string sector) : base(ArticleType.Sector, headline, content)
    {
        Sector = sector;
    }

    public string Sector { get; init; } = null!;
    
    public static Result<SectorArticle> Create(string headline, string content, string sector)
    {
        var article = new SectorArticle(headline, content, sector);

        return Result<SectorArticle>.Success(article);
    }
}
