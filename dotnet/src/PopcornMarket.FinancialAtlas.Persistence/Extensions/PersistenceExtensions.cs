using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
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
        EntityMap.Configure();
        BalanceSheetMap.Configure();
        CashFlowStatementMap.Configure();
        CompanyMap.Configure();
        FinancialStatementMap.Configure();
        IncomeStatementMap.Configure();
        MarketDataMap.Configure();
        MarketSnapshotMap.Configure();
        
        var context = new MongoDbContext(connectionString, DbConstants.DatabaseName);
        
        var companiesCollection = context.GetCollection<Company>(DbConstants.CompanyCollection);

        // Ensure text index on Name and Industry
        var indexKeys = Builders<Company>.IndexKeys.Text(c => c.Name).Text(c => c.Industry);
        var indexModel = new CreateIndexModel<Company>(indexKeys);
        companiesCollection.Indexes.CreateOne(indexModel);
        
        services.AddSingleton(context);
    }
    
    private static void AddClassesWithLifetime(IServiceCollection services)
    {
        services.AddScoped<ICompanyRepository, CompanyRepository>();
    }

}
