namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface ILoginService
{
    Task<AuthenticatedUserResponse> LoginAsync(ClaimsPrincipal user);
}
