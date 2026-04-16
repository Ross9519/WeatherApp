using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class ForecastResult
{
    [JsonPropertyName("current_weather")]
    public CurrentWeather? CurrentWeather { get; set; }
}