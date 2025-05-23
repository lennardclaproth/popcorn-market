﻿using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Domain.Abstractions;

public interface IMacroEconomicArticleRepository : IRepository<MacroEconomicArticle>
{
    Task<IEnumerable<MacroEconomicArticle>> GetArticlesByRegion(string region, int limit);
}
