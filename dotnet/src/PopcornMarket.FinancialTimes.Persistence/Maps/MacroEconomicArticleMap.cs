using MongoDB.Bson.Serialization;
using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Persistence.Maps;

internal static class MacroEconomicArticleMap
{
    internal static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(MacroEconomicArticle)))
        {
            BsonClassMap.RegisterClassMap<MacroEconomicArticle>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(c => c.Region).SetElementName("region");
            });
        }
    }
}
