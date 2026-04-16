using System.Text.Json;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations;

public class GeocodingService(HttpClient httpClient, IConfiguration configuration) : IGeocodingService
{
    public async Task<ServiceResult<List<CitySearchResult>>> SearchCitiesAsync(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return ServiceResult<List<CitySearchResult>>.Fail("City name is empty.");

        try
        {
            var baseUrl = configuration["OpenMeteo:GeocodingUrl"];
            var language = configuration["OpenMeteo:Language"] ?? "en";

            var url =
                $"{baseUrl}?name={Uri.EscapeDataString(cityName)}" +
                $"&language={language}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return ServiceResult<List<CitySearchResult>>.Fail("Geocoding API error.");

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GeocodingApiResponse>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var data = result?.Results ?? [];

            if (data.Count == 0)
                return ServiceResult<List<CitySearchResult>>.Fail("No cities found.");

            return ServiceResult<List<CitySearchResult>>.Ok(data);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<List<CitySearchResult>>.Fail("Network error while calling geocoding service.");
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<List<CitySearchResult>>.Fail("Geocoding request timed out.");
        }
        catch
        {
            return ServiceResult<List<CitySearchResult>>.Fail("Unexpected error occurred.");
        }
    }
}