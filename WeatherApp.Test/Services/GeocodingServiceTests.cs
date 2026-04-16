using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using WeatherApp.Services.Implementations;
using WeatherApp.Tests.TestUtils;

namespace WeatherApp.Tests.Services;

public class GeocodingServiceTests
{
    private static IConfiguration CreateConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenMeteo:GeocodingUrl"] = "https://fake-api.com/search",
                ["OpenMeteo:Language"] = "en"
            })
            .Build();
    }

    private static GeocodingService CreateSut(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handler = MockHttpHandler.CreateJson(json, status);
        var httpClient = new HttpClient(handler);

        return new GeocodingService(httpClient, CreateConfig());
    }

    // =========================
    // GREEN CASE
    // =========================

    [Fact]
    public async Task SearchCities_ReturnsSuccess_WhenApiReturnsData()
    {
        // Arrange
        var sut = CreateSut(TestDataBuilder.GeocodingSuccessJson);

        // Act
        var result = await sut.SearchCitiesAsync("Padova");

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("Padova");
    }

    // =========================
    // EMPTY RESULTS
    // =========================

    [Fact]
    public async Task SearchCities_ReturnsFailure_WhenNoCitiesFound()
    {
        // Arrange
        var sut = CreateSut(TestDataBuilder.GeocodingEmptyJson);

        // Act
        var result = await sut.SearchCitiesAsync("UnknownCity");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("No cities found.");
    }

    // =========================
    // API FAILURE
    // =========================

    [Fact]
    public async Task SearchCities_ReturnsFailure_WhenApiFails()
    {
        // Arrange
        var sut = CreateSut("", HttpStatusCode.InternalServerError);

        // Act
        var result = await sut.SearchCitiesAsync("Padova");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Geocoding API error.");
    }

    // =========================
    // INVALID INPUT
    // =========================

    [Fact]
    public async Task SearchCities_ReturnsFailure_WhenCityNameIsEmpty()
    {
        // Arrange
        var sut = CreateSut(TestDataBuilder.GeocodingSuccessJson);

        // Act
        var result = await sut.SearchCitiesAsync("");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("City name is empty.");
    }

    // =========================
    // NULL RESPONSE SAFETY
    // =========================

    [Fact]
    public async Task SearchCities_ReturnsFailure_WhenResponseIsNull()
    {
        // Arrange
        var sut = CreateSut("{}", HttpStatusCode.OK);

        // Act
        var result = await sut.SearchCitiesAsync("Padova");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("No cities found.");
    }
}