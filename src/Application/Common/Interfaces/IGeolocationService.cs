// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Service for retrieving geolocation information based on IP addresses.
/// </summary>
public interface IGeolocationService : IService
{
    /// <summary>
    /// Gets the country code for the specified IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ISO 3166-1 alpha-2 country code, or null if lookup fails.</returns>
    Task<string?> GetCountryAsync(string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed geolocation information for the specified IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Geolocation information, or null if lookup fails.</returns>
    Task<GeolocationInfo?> GetGeolocationAsync(string ipAddress, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents geolocation information for an IP address.
/// </summary>
public class GeolocationInfo
{
    /// <summary>
    /// Gets or sets the IP address.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ISO 3166-1 alpha-2 country code.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    public string? CountryName { get; set; }

    /// <summary>
    /// Gets or sets the region or state.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Gets or sets the timezone.
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Gets or sets the Internet Service Provider (ISP).
    /// </summary>
    public string? Isp { get; set; }

    /// <summary>
    /// Gets or sets the organization.
    /// </summary>
    public string? Organization { get; set; }
}
