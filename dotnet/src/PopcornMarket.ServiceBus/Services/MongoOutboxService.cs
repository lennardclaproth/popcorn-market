using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.ServiceBus.Models;
using MongoDB.Driver;
using System.Text.Json;

namespace PopcornMarket.ServiceBus.Services;

internal sealed class MongoOutboxService : IOutbox
{
    private readonly IMongoCollection<OutboxMessage> _collection;

    public MongoOutboxService(IMongoDatabase database, string collectionName = "outbox_messages")
    {
        _collection = database.GetCollection<OutboxMessage>(collectionName);

        // Optional: ensure indexes (idempotent)
        var indexKeys = Builders<OutboxMessage>.IndexKeys
            .Ascending(x => x.ProcessedAtUtc)
            .Ascending(x => x.CreatedAtUtc);

        var indexModel = new CreateIndexModel<OutboxMessage>(indexKeys);
        _collection.Indexes.CreateOne(indexModel);
    }

    public async Task EnqueueAsync(string topic, object payload, string? key = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);

        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Topic = topic,
            Payload = json,
            Key = key,
            CreatedAtUtc = DateTime.UtcNow,
            Attempts = 0
        };

        await _collection.InsertOneAsync(message, cancellationToken: cancellationToken);
        // If your domain is also in Mongo and you want true transactional outbox,
        // do this insert in the same Mongo transaction/session as your domain write.
    }

    public async Task<List<OutboxMessage>> GetUnprocessedBatchAsync(int take, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OutboxMessage>.Filter.And(
            Builders<OutboxMessage>.Filter.Eq(x => x.ProcessedAtUtc, null),
            Builders<OutboxMessage>.Filter.Lt(x => x.Attempts, 10)
        );

        var sort = Builders<OutboxMessage>.Sort.Ascending(x => x.CreatedAtUtc);

        return await _collection
            .Find(filter)
            .Sort(sort)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public Task MarkProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var update = Builders<OutboxMessage>.Update
            .Set(x => x.ProcessedAtUtc, DateTime.UtcNow);
        return _collection.UpdateOneAsync(x => x.Id == message.Id, update, cancellationToken: cancellationToken);
    }

    public Task MarkFailedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var update = Builders<OutboxMessage>.Update
            .Inc(x => x.Attempts, 1);
        return _collection.UpdateOneAsync(x => x.Id == message.Id, update, cancellationToken: cancellationToken);
    }
}
