using Blazor.Server.UI.Models.Authentication;

namespace Blazor.Server.UI.Services.Authentication;

public interface IAuthenticationService
{
    Task Login(LoginFormModel request);
    Task Logout();
}
