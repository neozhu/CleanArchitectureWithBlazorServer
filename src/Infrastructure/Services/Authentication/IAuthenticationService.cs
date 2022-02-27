using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public interface IAuthenticationService
{
    Task Login(LoginFormModel request);
    Task Logout();
}
