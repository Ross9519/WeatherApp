using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class CitySearchResult
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("admin1")]
    public string? Region { get; set; }

    [JsonPropertyName("admin2")]
    public string? Admin2 { get; set; }
}