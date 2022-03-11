namespace Blazor.Server.UI.Pages.Identity.Users;

public class UserFormModel
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Site { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? PhoneNumber { get; set; }
    public string[]? AssignRoles { get; set; }
    public bool Checked { get; set; }
}
