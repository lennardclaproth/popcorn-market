using System.Text.Json;
using StackExchange.Redis;

namespace PopcornMarket.OrderBook.Infrastructure.Caching;

internal sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _cache;
    
    public RedisCacheService(IDatabase cache)
    {
        _cache = cache;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.StringGetAsync(key);
        return json.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(json!);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.KeyDeleteAsync(key);
    }
}
