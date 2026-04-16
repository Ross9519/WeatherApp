using WeatherApp.Models; 

namespace WeatherApp.Tests.TestUtils;

public static class TestDataBuilder
{
    // =========================
    // GEOCODING JSON
    // =========================

    public static string GeocodingSuccessJson =>
        """
        {
            "results": [
                {
                    "name": "Padova",
                    "latitude": 45.4064,
                    "longitude": 11.8768,
                    "country": "Italy",
                    "admin1": "Veneto"
                }
            ]
        }
        """;

    public static string GeocodingEmptyJson =>
        """
        {
            "results": []
        }
        """;

    public static string GeocodingNullJson =>
        """
        {}
        """;

    public static string GeocodingInvalidJson =>
        """
        {
            "invalid": "structure"
        }
        """;

    // =========================
    // WEATHER JSON
    // =========================

    public static string WeatherSuccessJson =>
        """
        {
            "latitude": 45.4064,
            "longitude": 11.8768,
            "current_weather": {
                "temperature": 18.5,
                "windspeed": 6.2,
                "weathercode": 2,
                "time": "2026-04-15T12:00"
            }
        }
        """;

    // CASO EXPLICITO: field presente ma null (IMPORTANT per test service)
    public static string WeatherCurrentWeatherNullJson =>
        """
        {
            "latitude": 45.4064,
            "longitude": 11.8768,
            "current_weather": null
        }
        """;

    // CASO MISSING FIELD (resilienza deserialization)
    public static string WeatherMissingCurrentWeatherJson =>
        """
        {
            "latitude": 45.4064,
            "longitude": 11.8768
        }
        """;

    public static string WeatherInvalidJson =>
        """
        {
            "invalid": true
        }
        """;

    // =========================
    // DOMAIN OBJECTS
    // =========================

    public static CitySearchResult SampleCity => new()
    {
        Name = "Padova",
        Latitude = 45.4064,
        Longitude = 11.8768,
        Country = "Italy",
        Region = "Veneto"
    };

    public static WeatherResult SampleWeather => new()
    {
        CityName = "Padova",
        Region = "Veneto",
        Country = "Italy",
        Latitude = 45.4064,
        Longitude = 11.8768,
        Temperature = 18.5,
        WindSpeed = 6.2,
        WeatherCode = 2,
        Time = DateTime.Parse("2026-04-15T12:00:00")
    };
}