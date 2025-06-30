// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace CleanArchitecture.Blazor.Infrastructure.UnitTests.Services;

public class GeolocationServiceTests
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger<GeolocationService> _logger;
    private readonly GeolocationService _service;

    public GeolocationServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = new TestLogger<GeolocationService>(output);
        
        var httpClient = new HttpClient();
        _service = new GeolocationService(httpClient, _logger);
    }

    [Fact]
    public async Task GetCountryAsync_WithValidIP_ReturnsCountryCode()
    {
        // Arrange
        var ipAddress = "8.8.8.8"; // Google DNS

        // Act
        var result = await _service.GetCountryAsync(ipAddress);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        _output.WriteLine($"Country for {ipAddress}: {result}");
    }

    [Fact]
    public async Task GetCountryAsync_WithInvalidIP_ReturnsNull()
    {
        // Arrange
        var ipAddress = "invalid-ip";

        // Act
        var result = await _service.GetCountryAsync(ipAddress);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCountryAsync_WithEmptyIP_ReturnsNull()
    {
        // Arrange
        var ipAddress = "";

        // Act
        var result = await _service.GetCountryAsync(ipAddress);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetGeolocationAsync_WithValidIP_ReturnsGeolocationInfo()
    {
        // Arrange
        var ipAddress = "24.48.0.1"; // Test IP from your example

        // Act
        var result = await _service.GetGeolocationAsync(ipAddress);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ipAddress, result.IpAddress);
        Assert.NotNull(result.Country);
        Assert.NotEmpty(result.Country);
        
        _output.WriteLine($"Geolocation for {ipAddress}:");
        _output.WriteLine($"  Country: {result.Country}");
        _output.WriteLine($"  Country Name: {result.CountryName}");
        _output.WriteLine($"  Region: {result.Region}");
        _output.WriteLine($"  City: {result.City}");
        _output.WriteLine($"  Postal Code: {result.PostalCode}");
        _output.WriteLine($"  Latitude: {result.Latitude}");
        _output.WriteLine($"  Longitude: {result.Longitude}");
        _output.WriteLine($"  Timezone: {result.Timezone}");
        _output.WriteLine($"  ISP: {result.Isp}");
        _output.WriteLine($"  Organization: {result.Organization}");
    }

    [Theory]
    [InlineData("1.1.1.1")]     // Cloudflare DNS
    [InlineData("208.67.222.222")] // OpenDNS
    public async Task GetCountryAsync_WithVariousValidIPs_ReturnsCountryCode(string ipAddress)
    {
        // Act
        var result = await _service.GetCountryAsync(ipAddress);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        _output.WriteLine($"Country for {ipAddress}: {result}");
    }
}

/// <summary>
/// Simple test logger implementation for unit tests.
/// </summary>
public class TestLogger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _output;

    public TestLogger(ITestOutputHelper output)
    {
        _output = output;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _output.WriteLine($"[{logLevel}] {formatter(state, exception)}");
        if (exception != null)
        {
            _output.WriteLine(exception.ToString());
        }
    }
}
