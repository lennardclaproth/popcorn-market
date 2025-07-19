using MongoDB.Bson.Serialization;
using Popcorn.FinancialAtlas.Domain.ValueObjects;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

internal static class ReportingPeriodMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(ReportingPeriod)))
        {
            BsonClassMap.RegisterClassMap<ReportingPeriod>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                
                cm.MapMember(c => c.Type)
                    .SetElementName("type");
                
                cm.MapMember(c => c.Year)
                    .SetElementName("year");
                
                cm.MapMember(c => c.PeriodNumber)
                    .SetElementName("period_number");
            });
        }
    }
}
