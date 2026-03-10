using System.Collections.Concurrent;
using System.Text.Json;
using Shop_Cam_BE.Application.Common.Interfaces;

namespace Shop_Cam_BE.Infrastructure.Services;

/// <summary>
/// In-memory stub for Redis when ConnectionStrings:Redis is not set (e.g. local dev).
/// Not distributed and not persistent across restarts.
/// </summary>
public class InMemoryRedisStub : IRedisService
{
    private static readonly ConcurrentDictionary<string, (string Value, DateTime? Expiry)> Store = new();

    public Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var expiryAt = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : (DateTime?)null;
        Store[key] = (value, expiryAt);
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string key)
    {
        if (!Store.TryGetValue(key, out var entry))
            return Task.FromResult<string?>(null);
        if (entry.Expiry.HasValue && DateTime.UtcNow > entry.Expiry.Value)
        {
            Store.TryRemove(key, out _);
            return Task.FromResult<string?>(null);
        }
        return Task.FromResult<string?>(entry.Value);
    }

    public Task RemoveAsync(string key)
    {
        Store.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        return SetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await GetAsync(key);
        if (string.IsNullOrEmpty(json)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }
}
