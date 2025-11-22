using MongoDB.Bson.Serialization;
using Popcorn.FinancialAtlas.Domain.ValueObjects;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;
public class AnalysisMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Analysis)))
        {
            BsonClassMap.RegisterClassMap<Analysis>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(b => b.Ticker).SetElementName("ticker");
                cm.MapMember(b => b.TargetPrice).SetElementName("target_price");
                cm.MapMember(b => b.Current).SetElementName("current");
                cm.MapMember(b => b.OneWeek).SetElementName("1w");
                cm.MapMember(b => b.OneMonth).SetElementName("1m");
                cm.MapMember(b => b.ThreeMonths).SetElementName("3m");
                cm.MapMember(b => b.Date).SetElementName("date");
            });
        }
    }
}
