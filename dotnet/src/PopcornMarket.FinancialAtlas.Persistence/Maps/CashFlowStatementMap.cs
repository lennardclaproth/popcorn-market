using MongoDB.Bson.Serialization;
using Popcorn.FinancialAtlas.Domain.ValueObjects;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

internal static class CashFlowStatementMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(CashFlowStatement)))
        {
            BsonClassMap.RegisterClassMap<CashFlowStatement>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(c => c.OperatingCashFlowB)
                    .SetElementName("operating_cash_flow_b");
                
                cm.MapMember(c => c.CapitalExpendituresB)
                    .SetElementName("capital_expenditures_b");
                
                cm.MapMember(c => c.FreeCashFlowB)
                    .SetElementName("free_cash_flow_b");
                
                cm.MapMember(c => c.FinancingCashFlowB)
                    .SetElementName("financing_cash_flow_b");
                
                cm.MapMember(c => c.InvestingCashFlowB)
                    .SetElementName("investing_cash_flow_b");
                
                cm.MapMember(c => c.NetCashFlowB)
                    .SetElementName("net_cash_flow_b");
            });
        }
    }
}
