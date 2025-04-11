namespace Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;

public interface IRedisCacheService
{
    Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
    Task<T> GetCacheValueAsync<T>(string key);
    Task<bool> DeleteKeyAsync(string key);
}
