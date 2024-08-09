namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

/// <summary>
///     Represents the privacy settings for the application.
/// </summary>
public class PrivacySettings
{
    /// <summary>
    ///     Gets the unique key to identify the PrivacySettings configuration.
    /// </summary>
    public const string Key = nameof(PrivacySettings);

    /// <summary>
    ///     Gets or sets a value indicating whether the client IP addresses should be logged.
    /// </summary>
    public bool LogClientIpAddresses { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the client agents (user agents) should be logged.
    /// </summary>
    public bool LogClientAgents { get; set; } = true;

}