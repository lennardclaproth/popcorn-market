using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

internal static class FinancialStatementMap
{
    internal static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(FinancialStatement)))
        {
            // Create custom class map
            BsonClassMap.RegisterClassMap<FinancialStatement>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(fs => fs.TickerSymbol)
                    .SetElementName("ticker_symbol");

                cm.MapMember(fs => fs.Year)
                    .SetElementName("year");

                cm.MapMember(fs => fs.StatementInterval)
                    .SetElementName("statement_interval");

                cm.MapMember(fs => fs.IncomeStatement)
                    .SetElementName("income_statement");

                cm.MapMember(fs => fs.BalanceSheet)
                    .SetElementName("balance_sheet");

                cm.MapMember(fs => fs.CashFlowStatement)
                    .SetElementName("cash_flow_statement");
            });
        }
    }
}
