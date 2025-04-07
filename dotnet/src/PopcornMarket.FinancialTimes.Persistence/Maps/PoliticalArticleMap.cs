using MongoDB.Bson.Serialization;
using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Persistence.Maps;

internal static class PoliticalArticleMap
{
    internal static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(PoliticalArticle)))
        {
            BsonClassMap.RegisterClassMap<PoliticalArticle>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                    
                cm.MapMember(c => c.Region).SetElementName("region");
            });
        }
    }
}
