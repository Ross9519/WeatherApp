using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces;

public interface IWeatherService
{
    Task<ServiceResult<WeatherResult>> GetWeatherAsync(CitySearchResult city);
}