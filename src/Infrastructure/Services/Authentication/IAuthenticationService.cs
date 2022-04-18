using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public interface IAuthenticationService
{
    Task<bool> Login(LoginFormModel request);
    Task<bool> ExternalLogin(string provider, string userName, string name, string accessToken);
    Task Logout();
}
