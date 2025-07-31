namespace CleanArchitecture.Blazor.Application.Common.Security;

/// <summary>
/// Immutable user profile record representing user information state.
/// </summary>
public sealed record UserProfile(
    string UserId,
    string UserName,
    string Email,
    string? Provider = null,
    string? SuperiorName = null,
    string? SuperiorId = null,
    string? ProfilePictureDataUrl = null,
    string? DisplayName = null,
    string? PhoneNumber = null,
    string? DefaultRole = null,
    string[]? AssignedRoles = null,
    bool IsActive = true,
    string? TenantId = null,
    string? TenantName = null,
    string? TimeZoneId = null,
    string? LanguageCode = null
)
{
    /// <summary>
    /// Gets the local time offset based on the user's time zone.
    /// </summary>
    public TimeSpan LocalTimeOffset => string.IsNullOrEmpty(TimeZoneId)
        ? TimeZoneInfo.Local.BaseUtcOffset
        : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId).BaseUtcOffset;

    /// <summary>
    /// Creates an empty user profile with default values.
    /// </summary>
    public static UserProfile Empty => new(
        UserId: string.Empty,
        UserName: string.Empty,
        Email: string.Empty
    );

    /// <summary>
    /// Checks if the user has the specified role.
    /// </summary>
    public bool IsInRole(string role) => AssignedRoles?.Contains(role) ?? false;
}

