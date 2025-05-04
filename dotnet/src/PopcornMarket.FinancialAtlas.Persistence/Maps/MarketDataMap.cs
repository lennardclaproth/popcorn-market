using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

internal static class MarketDataMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(MarketData)))
        {
            // Create custom class map for MarketData
            BsonClassMap.RegisterClassMap<MarketData>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(m => m.Ticker).SetElementName("ticker");
                cm.MapMember(m => m.SharesOutstanding).SetElementName("shares_outstanding");
                cm.MapMember(m => m.Current).SetElementName("current");
                cm.MapMember(m => m.History).SetElementName("history");
            });
        }
    }
}
