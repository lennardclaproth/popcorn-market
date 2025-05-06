using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace PopcornMarket.FinancialTimes.Persistence.Context;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
        clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        var client = new MongoClient(clientSettings);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
}
