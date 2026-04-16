using FluentAssertions;
using Moq;
using WeatherApp.Models;
using WeatherApp.Orchestrators.Implementations;
using WeatherApp.Services.Interfaces;
using WeatherApp.Cache.Services;

namespace WeatherApp.Tests.Orchestrator;

public class WeatherOrchestratorTests
{
    private readonly Mock<IGeocodingService> _geoMock = new();
    private readonly Mock<IWeatherService> _weatherMock = new();
    private readonly Mock<IWeatherCacheService> _cacheMock = new();

    private readonly WeatherOrchestrator _sut;

    public WeatherOrchestratorTests()
    {
        _sut = new WeatherOrchestrator(
            _geoMock.Object,
            _weatherMock.Object,
            _cacheMock.Object);
    }

    // =========================
    // SEARCH CITIES
    // =========================

    [Fact]
    public async Task SearchCities_ReturnsSuccess_WhenServiceReturnsData()
    {
        // Arrange
        var cities = new List<CitySearchResult>
        {
            new() { Name = "Padova", Latitude = 45, Longitude = 11 }
        };

        _geoMock
            .Setup(x => x.SearchCitiesAsync("Padova"))
            .ReturnsAsync(ServiceResult<List<CitySearchResult>>.Ok(cities));

        // Act
        var result = await _sut.SearchCitiesAsync("Padova");

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchCities_ReturnsFailure_WhenServiceFails()
    {
        // Arrange
        _geoMock
            .Setup(x => x.SearchCitiesAsync(It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<List<CitySearchResult>>.Fail("error"));

        // Act
        var result = await _sut.SearchCitiesAsync("Padova");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    // =========================
    // WEATHER CACHE HIT
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsCachedValue_WhenExists()
    {
        // Arrange
        var city = new CitySearchResult
        {
            Name = "Padova",
            Latitude = 45,
            Longitude = 11
        };

        var cached = new WeatherResult
        {
            CityName = "Padova",
            Temperature = 20
        };

        _cacheMock
            .Setup(x => x.TryGet(It.IsAny<string>(), out cached))
            .Returns(true);

        // Act
        var result = await _sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Temperature.Should().Be(20);

        _weatherMock.Verify(
            x => x.GetWeatherAsync(It.IsAny<CitySearchResult>()),
            Times.Never);
    }

    // =========================
    // WEATHER CACHE MISS
    // =========================

    [Fact]
    public async Task GetWeather_CallsWeatherService_WhenCacheMiss()
    {
        // Arrange
        var city = new CitySearchResult
        {
            Name = "Padova",
            Latitude = 45,
            Longitude = 11
        };

        var weather = new WeatherResult
        {
            CityName = "Padova",
            Temperature = 25
        };

        WeatherResult outCache;
        _cacheMock
            .Setup(x => x.TryGet(It.IsAny<string>(), out outCache))
            .Returns(false);

        _weatherMock
            .Setup(x => x.GetWeatherAsync(city))
            .ReturnsAsync(ServiceResult<WeatherResult>.Ok(weather));

        // Act
        var result = await _sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Temperature.Should().Be(25);

        _weatherMock.Verify(x => x.GetWeatherAsync(city), Times.Once);
    }

    // =========================
    // EDGE CASES
    // =========================

    [Fact]
    public async Task GetWeather_ReturnsFailure_WhenCityIsNull()
    {
        // Act
        var result = await _sut.GetWeatherAsync(null!);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("City not selected.");
    }

    [Fact]
    public async Task GetWeather_DoesNotCache_WhenWeatherFails()
    {
        // Arrange
        var city = new CitySearchResult
        {
            Name = "Padova",
            Latitude = 45,
            Longitude = 11
        };

        _weatherMock
            .Setup(x => x.GetWeatherAsync(city))
            .ReturnsAsync(ServiceResult<WeatherResult>.Fail("API error"));

        // Act
        var result = await _sut.GetWeatherAsync(city);

        // Assert
        result.Success.Should().BeFalse();
    }
}