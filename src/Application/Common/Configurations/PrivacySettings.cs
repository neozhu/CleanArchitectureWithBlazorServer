namespace CleanArchitecture.Blazor.Application.Common.Configurations;

/// <summary>
///     Configuration wrapper for the privacy section
/// </summary>
public class PrivacySettings
{
    /// <summary>
    ///     Privacy key constraint
    /// </summary>
    public const string Key = nameof(PrivacySettings);
    
    /// <summary>
    ///     When enabled, the logs will include client IP addresses
    /// </summary>
    public bool LogClientIpAddresses { get; set; } = true;
    
    /// <summary>
    ///     When enabled, the logs will include the client agents
    /// </summary>
    public bool LogClientAgents { get; set; } = true;
}