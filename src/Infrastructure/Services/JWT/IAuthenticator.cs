using CleanArchitecture.Blazor.Domain.Features.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface IAuthenticator
{
    Task<ApplicationUser?> Authenticate(string username, string password);
    Task<string> Refresh(string refreshToken);
}
