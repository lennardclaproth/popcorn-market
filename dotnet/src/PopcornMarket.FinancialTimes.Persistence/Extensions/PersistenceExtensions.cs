using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Persistence.Constants;
using PopcornMarket.FinancialTimes.Persistence.Context;
using PopcornMarket.FinancialTimes.Persistence.Maps;
using PopcornMarket.FinancialTimes.Persistence.Repositories;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialTimes.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection InstallPersistence(this IServiceCollection services, string connectionString)
    {
        InitializeMongoDb(services, connectionString);
        SetupRepositories(services);

        return services;
    }

    private static void InitializeMongoDb(IServiceCollection services, string connectionString)
    {
        EntityMap.Configure();
        ArticleMap.Configure();
        ArticleTypeMap.Configure();
        CompanyArticleMap.Configure();
        SectorArticleMap.Configure();
        PoliticalArticleMap.Configure();
        MacroEconomicArticleMap.Configure();
        CompanyMap.Configure();
        
        var context = new MongoDbContext(connectionString, DbConstants.DatabaseName);

        services.AddSingleton(context);
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<ICompanyArticleRepository, CompanyArticleRepository>();
        services.AddScoped<IMacroEconomicArticleRepository, MacroEconomicArticleRepository>();
        services.AddScoped<ISectorArticleRepository, SectorArticleRepository>();
        services.AddScoped<IPoliticalArticleRepository, PoliticalArticleRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
    }
}
