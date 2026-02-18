using CleanArchitecture.Blazor.Server.UI.Middlewares;

namespace CleanArchitecture.Blazor.Server.UI.Extensions;

public static class StaleCookieMiddlewareExtensions
{
    public static IApplicationBuilder UseDataProtectionKeyCheck(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<StaleCookieMiddleware>();
    }

    public static IServiceCollection AddDataProtectionKeyCheck(
        this IServiceCollection services, Action<StaleCookieOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<StaleCookieOptions>(_ => { });
        }
        return services;
    }
}
