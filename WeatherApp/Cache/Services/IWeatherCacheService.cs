using WeatherApp.Models;

namespace WeatherApp.Cache.Services;

public interface IWeatherCacheService
{
    bool TryGet(string cityKey, out WeatherResult result);

    void Set(string cityKey, WeatherResult result);

    void Remove(string cityKey);
}