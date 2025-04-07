using MongoDB.Bson.Serialization;
using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Persistence.Maps;

internal static class CompanyArticleMap
{
    internal static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(CompanyArticle)))
        {
            BsonClassMap.RegisterClassMap<CompanyArticle>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                    
                cm.MapMember(c => c.Ticker).SetElementName("ticker");
                cm.MapMember(c => c.Sector).SetElementName("sector");
                cm.MapMember(c => c.CompanyName).SetElementName("company_name");
            });
        }
    }
}
