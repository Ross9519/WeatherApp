using WeatherApp.Cache.Services;
using WeatherApp.Models;
using WeatherApp.Orchestrators.Interfaces;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Orchestrators.Implementations;

public class WeatherOrchestrator(
    IGeocodingService geocodingService,
    IWeatherService weatherService,
    IWeatherCacheService cacheService) : IWeatherOrchestrator
{
    public Task<ServiceResult<List<CitySearchResult>>> SearchCitiesAsync(string cityName)
        => geocodingService.SearchCitiesAsync(cityName);

    public async Task<ServiceResult<WeatherResult>> GetWeatherAsync(CitySearchResult city)
    {
        if (city == null)
            return ServiceResult<WeatherResult>.Fail("City not selected.");

        var cacheKey = BuildCacheKey(city);

        if (cacheService.TryGet(cacheKey, out var cached))
            return ServiceResult<WeatherResult>.Ok(cached);

        var weatherResult = await weatherService.GetWeatherAsync(city);

        if (!weatherResult.Success || weatherResult.Data == null)
            return weatherResult;

        cacheService.Set(cacheKey, weatherResult.Data);

        return weatherResult;
    }

    private static string BuildCacheKey(CitySearchResult city)
    {
        return $"{city.Name?.ToLowerInvariant()}_{city.Latitude}_{city.Longitude}";
    }
}