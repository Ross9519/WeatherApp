using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using WeatherApp.Services.Implementations;
using WeatherApp.Tests.TestUtils;

namespace WeatherApp.Tests.Services;

public class WeatherServiceTests
{
    private static IConfiguration CreateConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenMeteo:ForecastUrl"] = "https://fake-api.com/forecast"
            })
            .Build();
    }

    private static WeatherService CreateSut(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handler = MockHttpHandler.CreateJson(json, status);
        var httpClient = new HttpClient(handler);

        return new WeatherService(httpClient, CreateConfig());
    }

    // =========================
    // GREEN CASE
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsSuccess_WhenApiReturnsValidData()
    {
        // Arrange
        var sut = CreateSut(TestDataBuilder.WeatherSuccessJson);

        var city = TestDataBuilder.SampleCity;

        // Act
        var result = await sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Temperature.Should().Be(18.5);
        result.Data!.WindSpeed.Should().Be(6.2);
        result.Data!.WeatherCode.Should().Be(2);
    }

    // =========================
    // NULL CITY
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsFailure_WhenCityIsNull()
    {
        // Arrange
        var sut = CreateSut(TestDataBuilder.WeatherSuccessJson);

        // Act
        var result = await sut.GetWeatherAsync(null!);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("City is null.");
    }

    // =========================
    // API FAILURE
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsFailure_WhenApiFails()
    {
        // Arrange
        var sut = CreateSut("", HttpStatusCode.InternalServerError);

        var city = TestDataBuilder.SampleCity;

        // Act
        var result = await sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Weather API error.");
    }

    // =========================
    // EMPTY FORECAST RESPONSE
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsFailure_WhenCurrentWeatherMissing()
    {
        // Arrange
        var sut = CreateSut(TestDataBuilder.WeatherMissingCurrentWeatherJson);

        var city = TestDataBuilder.SampleCity;

        // Act
        var result = await sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("No weather data returned.");
    }

    // =========================
    // EDGE CASE - INVALID JSON
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsFailure_WhenJsonIsInvalid()
    {
        // Arrange
        var sut = CreateSut("INVALID_JSON");

        var city = TestDataBuilder.SampleCity;

        // Act
        var result = await sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Unexpected error occurred.");
    }

    // =========================
    // EDGE CASE - HTTP TIMEOUT SIMULATION
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsFailure_WhenTimeoutOccurs()
    {
        // Arrange
        var handler = new TimeoutHttpHandler();
        var httpClient = new HttpClient(handler);

        var sut = new WeatherService(httpClient, CreateConfig());

        var city = TestDataBuilder.SampleCity;

        // Act
        var result = await sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Weather request timed out.");
    }
}