using FluentAssertions;
using Microsoft.Extensions.Configuration;
using WeatherApp.Cache.Services;
using WeatherApp.Models;

namespace WeatherApp.Tests.Services;

public class WeatherCacheServiceTests
{
    private static WeatherCacheService CreateSut(int ttlMinutes = 1)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cache:WeatherTtlMinutes"] = ttlMinutes.ToString()
            })
            .Build();

        return new WeatherCacheService(config);
    }

    private static WeatherResult SampleWeather => new()
    {
        CityName = "Padova",
        Temperature = 20
    };

    [Fact]
    public void Set_ThenGet_ReturnsCachedValue()
    {
        var sut = CreateSut();
        var key = "padova_45_11";

        sut.Set(key, SampleWeather);

        var exists = sut.TryGet(key, out var result);

        exists.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Temperature.Should().Be(20);
    }

    [Fact]
    public void TryGet_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var sut = CreateSut();

        var exists = sut.TryGet("unknown", out var result);

        exists.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void Set_OverwritesExistingValue()
    {
        var sut = CreateSut();
        var key = "padova_45_11";

        var first = new WeatherResult { CityName = "Padova", Temperature = 10 };
        var second = new WeatherResult { CityName = "Padova", Temperature = 25 };

        sut.Set(key, first);
        sut.Set(key, second);

        sut.TryGet(key, out var result);

        result!.Temperature.Should().Be(25);
    }

    [Fact]
    public void Set_IgnoresEmptyKey()
    {
        var sut = CreateSut();

        sut.Set("", SampleWeather);

        var exists = sut.TryGet("", out _);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Cache_Expires_AfterTTL()
    {
        var sut = CreateSut(ttlMinutes: 0);
        var key = "padova_45_11";

        sut.Set(key, SampleWeather);

        await Task.Delay(1100);

        var exists = sut.TryGet(key, out var result);

        exists.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void Cache_Is_Isolated_BetweenKeys()
    {
        var sut = CreateSut();

        sut.Set("padova", new WeatherResult { CityName = "Padova", Temperature = 20 });
        sut.Set("milano", new WeatherResult { CityName = "Milano", Temperature = 10 });

        sut.TryGet("padova", out var p);
        sut.TryGet("milano", out var m);

        p!.Temperature.Should().Be(20);
        m!.Temperature.Should().Be(10);
    }
}