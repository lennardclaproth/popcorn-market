using MongoDB.Driver;

namespace PopcornMarket.FinancialTimes.Persistence.Context;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
}
