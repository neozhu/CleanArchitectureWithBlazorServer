// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

/// <summary>
/// Service for handling login audit operations with geolocation enrichment.
/// </summary>
public class LoginAuditService
{
    private readonly IGeolocationService _geolocationService;
    private readonly ILogger<LoginAuditService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginAuditService"/> class.
    /// </summary>
    /// <param name="geolocationService">The geolocation service.</param>
    /// <param name="logger">The logger instance.</param>
    public LoginAuditService(
        IGeolocationService geolocationService,
        ILogger<LoginAuditService> logger)
    {
        _geolocationService = geolocationService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new login audit entry with geolocation information.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="ipAddress">The IP address of the login attempt.</param>
    /// <param name="browserInfo">Browser information.</param>
    /// <param name="provider">Authentication provider.</param>
    /// <param name="success">Whether the login was successful.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A login audit entry enriched with geolocation information.</returns>
    public async Task<LoginAudit> CreateLoginAuditAsync(
        string userId,
        string userName,
        string? ipAddress,
        string? browserInfo = null,
        string? provider = null,
        bool success = true,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating login audit for user {UserName} from IP {IpAddress}", userName, ipAddress);

        var loginAudit = new LoginAudit
        {
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            BrowserInfo = browserInfo,
            Provider = provider,
            Success = success,
            LoginTimeUtc = DateTimeOffset.UtcNow
        };

        // Enrich with geolocation information if IP address is available
        if (!string.IsNullOrWhiteSpace(ipAddress))
        {
            try
            {
                await loginAudit.EnrichWithGeolocationAsync(_geolocationService, cancellationToken);
                _logger.LogDebug("Login audit enriched with geolocation: {Region}", loginAudit.Region);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to enrich login audit with geolocation for IP {IpAddress}", ipAddress);
            }
        }

        return loginAudit;
    }

    /// <summary>
    /// Updates an existing login audit with geolocation information.
    /// </summary>
    /// <param name="loginAudit">The login audit to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated login audit.</returns>
    public async Task<LoginAudit> UpdateWithGeolocationAsync(
        LoginAudit loginAudit, 
        CancellationToken cancellationToken = default)
    {
        if (loginAudit == null)
            throw new ArgumentNullException(nameof(loginAudit));

        _logger.LogDebug("Updating login audit {LoginAuditId} with geolocation for IP {IpAddress}", 
            loginAudit.Id, loginAudit.IpAddress);

        await loginAudit.EnrichWithGeolocationAsync(_geolocationService, cancellationToken);

        return loginAudit;
    }

    /// <summary>
    /// Gets geolocation statistics for login audits.
    /// </summary>
    /// <param name="loginAudits">Collection of login audits.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary of country codes and their login counts.</returns>
    public async Task<Dictionary<string, int>> GetLoginStatsByCountryAsync(
        IEnumerable<LoginAudit> loginAudits,
        CancellationToken cancellationToken = default)
    {
        var countryStats = new Dictionary<string, int>();

        foreach (var audit in loginAudits.Where(a => !string.IsNullOrWhiteSpace(a.IpAddress)))
        {
            try
            {
                var country = await audit.GetCountryAsync(_geolocationService, cancellationToken);
                if (!string.IsNullOrEmpty(country))
                {
                    countryStats[country] = countryStats.GetValueOrDefault(country, 0) + 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get country for login audit {LoginAuditId}", audit.Id);
            }
        }

        return countryStats;
    }
}
