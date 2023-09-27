using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Versioning;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class JWTServicesCollectionExtensions
{
    public static IServiceCollection AddSimpleJwtService(this IServiceCollection services, Action<SimpleJwtOptions> options)
    {
        var sjOptions = new SimpleJwtOptions();

        if (options != null)
        {
            options.Invoke(sjOptions);
        }
        services.AddSingleton(typeof(IOptions<SimpleJwtOptions>), Options.Create(sjOptions));
        services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
        services.AddScoped<ITokenValidator, TokenValidator>();
        services.AddScoped<ILoginService, JwtLoginService>();
        services.AddScoped<JwtSecurityTokenHandler>();
        return services;
    }
}