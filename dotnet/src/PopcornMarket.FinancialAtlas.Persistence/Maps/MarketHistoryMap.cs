using MongoDB.Bson.Serialization;
using Popcorn.FinancialAtlas.Domain.ValueObjects;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

internal static class MarketHistoryMap
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
