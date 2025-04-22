using Capstone.ECommerceApp.Domain.Core.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace Capstone.ECommerceApp.Infra.RedisCache;


public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetCacheValueAsync<T>(string key, string hashField, T value)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.HashSetAsync(key, hashField, json);
    }

    public async Task<T> GetCacheValueAsync<T>(string key, string hashField)
    {
        var db = _redis.GetDatabase();
        var json = await db.HashGetAsync(key, hashField);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public async Task<bool> DeleteKeyAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.KeyDeleteAsync(key);
    }

    public async Task<bool> DeleteHashFieldAsync(string key, string hashField)
    {
        var db = _redis.GetDatabase();
        return await db.HashDeleteAsync(key, hashField);
    }
}

