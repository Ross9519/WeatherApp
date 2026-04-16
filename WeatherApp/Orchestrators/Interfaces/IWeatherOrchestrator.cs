using WeatherApp.Models;

namespace WeatherApp.Orchestrators.Interfaces;

public interface IWeatherOrchestrator
{
    Task<ServiceResult<List<CitySearchResult>>> SearchCitiesAsync(string cityName);

    Task<ServiceResult<WeatherResult>> GetWeatherAsync(CitySearchResult city);
}
