using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces;

public interface IGeocodingService
{
    Task<ServiceResult<List<CitySearchResult>>> SearchCitiesAsync(string cityName);
}