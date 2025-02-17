using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Persistence.Constants;
using PopcornMarket.FinancialAtlas.Persistence.Context;
using PopcornMarket.SharedKernel.Attributes.ServiceLifetime;
using Scrutor;

namespace PopcornMarket.FinancialAtlas.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        InitializeMongoDb(services, connectionString);
        AddClassesWithLifetime(services, PersistenceAssemblyReference.Assembly);
        
        return services;
    }

    private static void InitializeMongoDb(IServiceCollection services, string connectionString)
    {
        var context = new MongoDbContext(connectionString, DbConstants.DatabaseName);
        
        var companiesCollection = context.GetCollection<Company>(DbConstants.CompanyCollection);

        // Ensure text index on Name and Industry
        var indexKeys = Builders<Company>.IndexKeys.Text(c => c.Name).Text(c => c.Industry);
        var indexModel = new CreateIndexModel<Company>(indexKeys);
        companiesCollection.Indexes.CreateOne(indexModel);
        
        services.AddSingleton(context);
    }
    
    private static void AddClassesWithLifetime(IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan => scan.FromAssemblies([assembly])
            .AddClasses(classes => classes.WithAttribute<SingletonLifetimeAttribute>()).AsImplementedInterfaces().WithSingletonLifetime()
            .AddClasses(classes => classes.WithAttribute<ScopedLifetimeAttribute>()).AsImplementedInterfaces().WithScopedLifetime()
            .AddClasses(classes => classes.WithAttribute<TransientLifetimeAttribute>()).AsImplementedInterfaces().WithTransientLifetime());
    }

}
