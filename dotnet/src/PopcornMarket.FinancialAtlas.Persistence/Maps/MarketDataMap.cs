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

                cm.MapMember(m => m.TickerSymbol).SetElementName("ticker_symbol");
                cm.MapMember(m => m.Current).SetElementName("current");
                cm.MapMember(m => m.History).SetElementName("history");
            });
        }
    }
}
