using Blazor.Analytics;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Constants.Localization;
using CleanArchitecture.Blazor.Server.Middlewares;
using CleanArchitecture.Blazor.Server.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddServer(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<LocalizationCookiesMiddleware>()
            .Configure<RequestLocalizationOptions>(options =>
            {
                options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.FallBackToParentUICultures = true;
            })
            .AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);

        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage())
            .AddHangfireServer()
            .AddMvc();

        services.AddControllers();

        services.AddScoped<IApplicationHubWrapper, ServerHubWrapper>()
            .AddSignalR();

        services.AddScoped<ExceptionHandlingMiddleware>()
            .AddHealthChecks();

        var privacySettings = config.GetRequiredSection(PrivacySettings.Key).Get<PrivacySettings>();
        if (privacySettings!.UseGoogleAnalytics)
        {
            if (privacySettings.GoogleAnalyticsKey is null or "")
            {
                throw new ArgumentNullException(nameof(privacySettings.GoogleAnalyticsKey));
            }

            services.AddGoogleAnalytics(privacySettings.GoogleAnalyticsKey);
        }

        return services;
    }
}