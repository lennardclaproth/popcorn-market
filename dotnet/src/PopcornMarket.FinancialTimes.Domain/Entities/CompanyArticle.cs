using PopcornMarket.FinancialTimes.Domain.Enums;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Domain.Entities;

public class CompanyArticle : Article
{
    public string Ticker { get; init; } = null!;
    public string Sector { get; init; } = null!;
    public string CompanyName { get; init; } = null!;
    
    private CompanyArticle(string headline, string content, string ticker, string sector, string companyName) : base(ArticleType.Company, headline, content)
    {
        Ticker = ticker;
        Sector = sector;
        CompanyName = companyName;
    }

    public static Result<CompanyArticle> Create(string headline, string content, string ticker,string sector,
        string companyName)
    {
        var article = new CompanyArticle(headline, content, ticker, sector, companyName);
        
        return Result<CompanyArticle>.Success(article);
    }
}
