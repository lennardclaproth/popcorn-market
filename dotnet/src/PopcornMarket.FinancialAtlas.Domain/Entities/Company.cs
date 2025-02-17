﻿using PopcornMarket.SharedKernel.Primitives;

namespace Popcorn.FinancialAtlas.Domain.Entities;

public class Company : Entity
{
    public string Ticker { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Industry { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Headquarters { get; private set; } = null!;
    public string Ceo { get; private set; } = null!;
    public int FoundedYear { get; private set; }
    public int Employees { get; private set; }
    
    private Company(){ }

    public Company(string ticker,
        string name,
        string industry,
        string description,
        string headquarters,
        string ceo,
        int foundedYear,
        int employees) : base(Guid.NewGuid())
    {
        Ticker = ticker;
        Name = name;
        Industry = industry;
        Description = description;
        Headquarters = headquarters;
        Ceo = ceo;
        FoundedYear = foundedYear;
        Employees = employees;
    }
}
