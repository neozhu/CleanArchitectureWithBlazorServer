namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public interface ITokenGeneratorService : IAccessTokenGenerator, IRefreshTokenGenerator
{
}
