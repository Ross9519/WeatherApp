using System.Text.Json;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations;


public class WeatherService(HttpClient httpClient, IConfiguration configuration) : IWeatherService
{
    public async Task<ServiceResult<WeatherResult>> GetWeatherAsync(CitySearchResult city)
    {
        if (city == null)
            return ServiceResult<WeatherResult>.Fail("City is null.");

        try
        {
            var baseUrl = configuration["OpenMeteo:ForecastUrl"];

            var url =
                $"{baseUrl}?latitude={city.Latitude}" +
                $"&longitude={city.Longitude}" +
                "&current_weather=true" +
                "&timezone=auto";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return ServiceResult<WeatherResult>.Fail("Weather API error.");

            var json = await response.Content.ReadAsStringAsync();

            var forecast = JsonSerializer.Deserialize<ForecastResult>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (forecast?.CurrentWeather == null)
                return ServiceResult<WeatherResult>.Fail("No weather data returned.");

            var utcTime = DateTime.SpecifyKind(
                forecast.CurrentWeather.Time,
                DateTimeKind.Utc
            );

            var tz = TimeZoneInfo.FindSystemTimeZoneById(forecast.CurrentWeather.TimeZone);

            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);

            var result = new WeatherResult
            {
                CityName = city.Name ?? "Unknown",
                Region = city.Region,
                Country = city.Country,
                Latitude = city.Latitude,
                Longitude = city.Longitude,
                Temperature = forecast.CurrentWeather.Temperature,
                WindSpeed = forecast.CurrentWeather.WindSpeed,
                WeatherCode = forecast.CurrentWeather.WeatherCode,
                Time = localTime
            };

            return ServiceResult<WeatherResult>.Ok(result);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<WeatherResult>.Fail("Network error while calling weather service.");
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<WeatherResult>.Fail("Weather request timed out.");
        }
        catch
        {
            return ServiceResult<WeatherResult>.Fail("Unexpected error occurred.");
        }
    }
}