using MongoDB.Bson.Serialization;
using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Persistence.Maps;

internal static class SectorArticleMap
{
    internal static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(SectorArticle)))
        {
            BsonClassMap.RegisterClassMap<SectorArticle>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                    
                cm.MapMember(c => c.Sector).SetElementName("sector");
                cm.MapMember(c => c.Region).SetElementName("region");
            });
        }
    }
}
