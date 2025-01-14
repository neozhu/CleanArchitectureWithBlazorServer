namespace CleanArchitecture.Blazor.Application.Common.Security;

public class PermissionModel
{
    public string Description { get; set; } = "Permission Description";
    public string Group { get; set; } = "Permission";
    public required string ClaimType { get; set; }
    public required string ClaimValue { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? HelpText { get; set; }
    public bool Assigned { get; set; }

    public string? RoleId { get; set; }
    public string? UserId { get; set; }
    public bool IsInherit { get; set; }
}