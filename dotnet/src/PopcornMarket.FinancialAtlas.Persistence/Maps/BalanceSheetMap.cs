using MongoDB.Bson.Serialization;
using Popcorn.FinancialAtlas.Domain.ValueObjects;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

public class BalanceSheetMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(BalanceSheet)))
        {
            BsonClassMap.RegisterClassMap<BalanceSheet>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(b => b.TotalAssetsB).SetElementName("total_assets_b");
                cm.MapMember(b => b.TotalLiabilitiesB).SetElementName("total_liabilities_b");
                cm.MapMember(b => b.TotalEquityB).SetElementName("total_equity_b");
                cm.MapMember(b => b.DebtToEquityRatio).SetElementName("debt_to_equity_ratio");
            });
        }
    }
}
