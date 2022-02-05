namespace MudDemo.Server.Models.Authentication
{
    public class LoginFormModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
