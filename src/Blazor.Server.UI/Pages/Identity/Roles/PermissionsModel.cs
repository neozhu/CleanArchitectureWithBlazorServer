namespace Blazor.Server.UI.Pages.Identity.Roles;

public class PermissionModel
{
    public string? Description { get; set; }
    public string? Group { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
    public bool Assigned { get; set; }

    public string? RoleId { get; set; }
    public string? UserId { get; set; }
}
