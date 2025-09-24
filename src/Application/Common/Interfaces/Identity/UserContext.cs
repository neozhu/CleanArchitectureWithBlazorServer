namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Represents the current user context with essential user information.
/// </summary>
public sealed record UserContext(
    string UserId,
    string UserName,
    string? DisplayName = null,
    string? TenantId = null,
    string? Email = null,
    IReadOnlyList<string>? Roles = null,
    string? ProfilePictureDataUrl = null,
    string? SuperiorId = null
); 
