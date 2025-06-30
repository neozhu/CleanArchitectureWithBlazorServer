// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

/// <summary>
/// Extension methods for login audit functionality.
/// </summary>
public static class LoginAuditExtensions
{
    /// <summary>
    /// Enriches the login audit with geolocation information based on the IP address.
    /// </summary>
    /// <param name="loginAudit">The login audit to enrich.</param>
    /// <param name="geolocationService">The geolocation service.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The enriched login audit with region information.</returns>
    public static async Task<LoginAudit> EnrichWithGeolocationAsync(
        this LoginAudit loginAudit, 
        IGeolocationService geolocationService, 
        CancellationToken cancellationToken = default)
    {
        if (loginAudit == null)
            throw new ArgumentNullException(nameof(loginAudit));

        if (geolocationService == null)
            throw new ArgumentNullException(nameof(geolocationService));

        if (string.IsNullOrWhiteSpace(loginAudit.IpAddress))
            return loginAudit;

        try
        {
            // Get geolocation information
            var geolocation = await geolocationService.GetGeolocationAsync(loginAudit.IpAddress, cancellationToken);
            
            if (geolocation != null)
            {
                // Build region string with available information
                var regionParts = new List<string>();
                
                if (!string.IsNullOrEmpty(geolocation.City))
                    regionParts.Add(geolocation.City);
                
                if (!string.IsNullOrEmpty(geolocation.Region))
                    regionParts.Add(geolocation.Region);
                
                if (!string.IsNullOrEmpty(geolocation.CountryName))
                    regionParts.Add(geolocation.CountryName);
                else if (!string.IsNullOrEmpty(geolocation.Country))
                    regionParts.Add(geolocation.Country);

                loginAudit.Region = regionParts.Count > 0 ? string.Join(", ", regionParts) : null;
            }
        }
        catch (Exception)
        {
            // Silently fail - geolocation is not critical for login audit
            // Logging should be handled by the GeolocationService itself
        }

        return loginAudit;
    }

    /// <summary>
    /// Gets only the country code for the login audit IP address.
    /// </summary>
    /// <param name="loginAudit">The login audit.</param>
    /// <param name="geolocationService">The geolocation service.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ISO 3166-1 alpha-2 country code, or null if lookup fails.</returns>
    public static async Task<string?> GetCountryAsync(
        this LoginAudit loginAudit, 
        IGeolocationService geolocationService, 
        CancellationToken cancellationToken = default)
    {
        if (loginAudit == null)
            throw new ArgumentNullException(nameof(loginAudit));

        if (geolocationService == null)
            throw new ArgumentNullException(nameof(geolocationService));

        if (string.IsNullOrWhiteSpace(loginAudit.IpAddress))
            return null;

        try
        {
            return await geolocationService.GetCountryAsync(loginAudit.IpAddress, cancellationToken);
        }
        catch (Exception)
        {
            // Silently fail - geolocation is not critical
            return null;
        }
    }
}
