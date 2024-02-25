namespace CleanArchitecture.Blazor.Application.Common.Security;

public class PermissionModel
{
    public string Description { get; set; } = "Permission Description";
    public string Group { get; set; } = "Permission";
    public string? ClaimType { get; set; }
    public string ClaimValue { get; set; } = "";
    public bool Assigned { get; set; }

    public string? RoleId { get; set; }
    public string? UserId { get; set; }
}