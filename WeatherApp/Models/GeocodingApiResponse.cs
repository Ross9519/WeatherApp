using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class GeocodingApiResponse
{
    [JsonPropertyName("results")]
    public List<CitySearchResult>? Results { get; set; }
}