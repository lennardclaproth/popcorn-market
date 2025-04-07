using MongoDB.Bson.Serialization;
using Popcorn.FinancialAtlas.Domain.ValueObjects;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

public class IncomeStatementMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(IncomeStatement)))
        {
            BsonClassMap.RegisterClassMap<IncomeStatement>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(i => i.RevenueB).SetElementName("revenue_b");
                cm.MapMember(i => i.CogsB).SetElementName("cogs_b");
                cm.MapMember(i => i.GrossProfitB).SetElementName("gross_profit_b");
                cm.MapMember(i => i.OperatingExpensesB).SetElementName("operating_expenses_b");
                cm.MapMember(i => i.EbitdaB).SetElementName("ebitda_b");
                cm.MapMember(i => i.DepreciationAmortizationB).SetElementName("depreciation_amortization_b");
                cm.MapMember(i => i.EbitB).SetElementName("ebit_b");
                cm.MapMember(i => i.InterestExpenseB).SetElementName("interest_expense_b");
                cm.MapMember(i => i.TaxRatePercent).SetElementName("tax_rate_percent");
                cm.MapMember(i => i.NetIncomeB).SetElementName("net_income_b");
                cm.MapMember(i => i.EpsUSD).SetElementName("eps_usd");
            });
        }
    }
}
