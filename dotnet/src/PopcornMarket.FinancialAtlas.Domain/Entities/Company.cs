using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Entities;

public class Company : AggregateRoot
{
    public string Ticker { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Industry { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Headquarters { get; private set; } = null!;
    public string Ceo { get; private set; } = null!;
    public int FoundedYear { get; private set; }
    public int Employees { get; private set; }
    public string Region { get; private set; }

    private Company(string ticker,
        string name,
        string industry,
        string description,
        string headquarters,
        string ceo,
        int foundedYear,
        int employees,
        string region) : base(Guid.NewGuid())
    {
        Ticker = ticker;
        Name = name;
        Industry = industry;
        Description = description;
        Headquarters = headquarters;
        Ceo = ceo;
        FoundedYear = foundedYear;
        Employees = employees;
        Region = region;
    }

    public static Result<Company> Create(string ticker, string name, string industry, string description,
        string headquarters, string ceo, int foundedYear, int employees, string region)
    {
        var company = new Company(ticker, name, industry, description, headquarters, ceo, foundedYear, employees, region);
        return Result<Company>.Success(company);
    }
}
