using PopcornMarket.FinancialTimes.Domain.Enums;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Domain.Entities;

public class SectorArticle : Article
{
    private SectorArticle(string headline, string content, string sector, string region) : base(ArticleType.Sector, headline, content)
    {
        Sector = sector;
        Region = region;
    }

    public string Sector { get; init; } = null!;
    public string Region { get; init; } = null!;
    
    public static Result<SectorArticle> Create(string headline, string content, string sector, string region)
    {
        var article = new SectorArticle(headline, content, sector, region);

        return Result<SectorArticle>.Success(article);
    }
}
