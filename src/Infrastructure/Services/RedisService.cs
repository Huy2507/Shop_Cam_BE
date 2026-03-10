using System.Text.Json;
using Shop_Cam_BE.Application.Common.Interfaces;
using StackExchange.Redis;

namespace Shop_Cam_BE.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        => await _db.StringSetAsync(key, value, expiry);

    public async Task<string?> GetAsync(string key)
        => await _db.StringGetAsync(key);

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch
        {
            return default;
        }
    }

    public async Task RemoveAsync(string key)
        => await _db.KeyDeleteAsync(key);
}
