namespace CleanArchitecture.Blazor.Application.Common.Models;

public class UserModel
{
    public string? Provider { get; set; }
    public string? Avatar { get; set; }
    public string? DisplayName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public string? UserId { get; set; }
    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}