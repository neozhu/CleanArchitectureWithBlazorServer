using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class UserProfile
{
    public string? Provider { get; set; }
    public string? SuperiorName { get; set; }
    public string? SuperiorId { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? DisplayName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DefaultRole { get; set; }
    public string[]? AssignedRoles { get; set; }
    public required string UserId { get; set; } = Guid.NewGuid().ToString();
    public bool IsActive { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
}
