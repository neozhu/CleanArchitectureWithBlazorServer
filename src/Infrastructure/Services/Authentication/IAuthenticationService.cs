using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public interface IAuthenticationService
{
    Task<bool> Login(LoginFormModel request);
    Task Logout();
}
