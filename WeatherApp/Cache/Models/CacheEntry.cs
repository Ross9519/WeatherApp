using WeatherApp.Models;

namespace WeatherApp.Cache.Models;

public class CacheEntry
{
    public WeatherResult Data { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}