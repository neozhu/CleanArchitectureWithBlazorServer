namespace Blazor.Server.UI.Models.Authentication
{
    public class LoginFormModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
