using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

internal static class MarketSnapshotMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(MarketSnapshot)))
        {
            BsonClassMap.RegisterClassMap<MarketSnapshot>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(ms => ms.StockPriceUSD).SetElementName("stock_price_usd");
                cm.MapMember(ms => ms.MarketCapBillion).SetElementName("market_cap_billion");
                cm.MapMember(ms => ms.DividendPerShareUSD).SetElementName("dividend_per_share_usd");
                cm.MapMember(ms => ms.DividendYieldPercent).SetElementName("dividend_yield_percent");
                cm.MapMember(ms => ms.Date).SetElementName("date");
            });
        }
    }
}
