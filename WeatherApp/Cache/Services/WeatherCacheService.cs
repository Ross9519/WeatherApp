using WeatherApp.Cache.Models;
using WeatherApp.Models;

namespace WeatherApp.Cache.Services;

public class WeatherCacheService : IWeatherCacheService
{
    private readonly Dictionary<string, CacheEntry> _cache = new();
    private readonly TimeSpan _ttl;
    private readonly object _lock = new();

    public WeatherCacheService(IConfiguration configuration)
    {
        var ttlMinutes = configuration.GetValue("Cache:WeatherTtlMinutes", 60);
        _ttl = TimeSpan.FromMinutes(ttlMinutes);
    }

    public bool TryGet(string cityKey, out WeatherResult result)
    {
        result = null!;

        if (string.IsNullOrWhiteSpace(cityKey))
            return false;

        lock (_lock)
        {
            if (!_cache.TryGetValue(cityKey, out var item))
                return false;

            if (DateTime.UtcNow - item.CreatedAt > _ttl)
            {
                _cache.Remove(cityKey);
                return false;
            }

            result = item.Data;
            return true;
        }
    }

    public void Set(string cityKey, WeatherResult result)
    {
        if (string.IsNullOrWhiteSpace(cityKey))
            return;

        lock (_lock)
        {
            _cache[cityKey] = new CacheEntry
            {
                Data = result,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public void Remove(string cityKey)
    {
        if (string.IsNullOrWhiteSpace(cityKey))
            return;

        lock (_lock)
        {
            _cache.Remove(cityKey);
        }
    }
}