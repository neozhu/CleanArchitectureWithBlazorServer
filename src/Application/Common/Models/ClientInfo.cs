namespace CleanArchitecture.Blazor.Application.Common.Models;

/// <summary>
/// Represents client connection information including IP address and browser details.
/// </summary>
public record ClientInfo
{
    /// <summary>
    /// Gets the client IP address.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// Gets the client browser user agent string.
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// Creates a new instance of ClientInfo.
    /// </summary>
    /// <param name="ipAddress">The client IP address.</param>
    /// <param name="userAgent">The browser user agent string.</param>
    public ClientInfo(string? ipAddress, string? userAgent)
    {
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    /// <summary>
    /// Creates an empty ClientInfo instance.
    /// </summary>
    public static ClientInfo Empty => new(null, null);
}
