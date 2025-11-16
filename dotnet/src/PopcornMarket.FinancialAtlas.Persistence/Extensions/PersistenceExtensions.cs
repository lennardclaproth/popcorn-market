using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;
using PopcornMarket.FinancialAtlas.Persistence.Maps;
using PopcornMarket.FinancialAtlas.Persistence.Repositories;

namespace PopcornMarket.FinancialAtlas.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection InstallPersistence(this IServiceCollection services, string connectionString)
    {
        InitializeMongoDb(services, connectionString);
        AddClassesWithLifetime(services);
        
        return services;
    }

    private static void InitializeMongoDb(IServiceCollection services, string connectionString)
    {
        ConfigureMappings();
        
        var context = new MongoDbContext(connectionString, DbConstants.DatabaseName);
        
        var companiesCollection = context.GetCollection<Company>(DbConstants.CompanyCollection);
        var historyCollection = context.GetCollection<MarketHistory>(DbConstants.MarketHistoryCollection);

        // Ensure text index on Name and Industry
        var indexKeys = Builders<Company>.IndexKeys.Text(c => c.Name).Text(c => c.Industry);
        var indexModel = new CreateIndexModel<Company>(indexKeys);
        companiesCollection.Indexes.CreateOne(indexModel);

        var historyIndexKeys = Builders<MarketHistory>.IndexKeys.Ascending(x => x.Ticker).Ascending(x => x.Date);
        var historyIndexModel = new CreateIndexModel<MarketHistory>(historyIndexKeys);
        historyCollection.Indexes.CreateOne(historyIndexModel);

        services.AddSingleton(context);
    }

    private static void ConfigureMappings()
    {
        EntityMap.Configure();
        BalanceSheetMap.Configure();
        CashFlowStatementMap.Configure();
        CompanyMap.Configure();
        FinancialStatementMap.Configure();
        IncomeStatementMap.Configure();
        MarketDataMap.Configure();
        MarketSnapshotMap.Configure();
        ReportingPeriodMap.Configure();
        AnalysisMap.Configure();
    }
    
    private static void AddClassesWithLifetime(IServiceCollection services)
    {
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IMarketDataRepository, MarketDataRepository>();
        services.AddScoped<IFinancialStatementRepository, FinancialStatementRepository>();
    }
}
