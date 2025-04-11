using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, expiration);
    }

    public async Task<T> GetCacheValueAsync<T>(string key)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public async Task<bool> DeleteKeyAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.KeyDeleteAsync(key);
    }
}
