namespace Capstone.ECommerceApp.Domain.Core.Cache;

public interface IRedisCacheService
{
    Task SetCacheValueAsync<T>(string key, string hashField, T value);
    Task<T> GetCacheValueAsync<T>(string key, string hashField);
    Task<bool> DeleteKeyAsync(string key);
}
