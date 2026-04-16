namespace WeatherApp.Models;

public class WeatherResult
{
    public string CityName { get; set; } = string.Empty;

    public string? Region { get; set; }

    public string? Country { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double Temperature { get; set; }

    public double WindSpeed { get; set; }

    public int WeatherCode { get; set; }

    public DateTime Time { get; set; }
}