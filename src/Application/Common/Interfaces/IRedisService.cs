namespace Shop_Cam_BE.Application.Common.Interfaces;

public interface IRedisService
{
    Task SetAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
}
