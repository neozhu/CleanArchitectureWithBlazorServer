using System.Reflection;
using Blazor.Analytics;
using Blazor.Server.UI.Middlewares;
using Blazor.Server.UI.Services.Layout;
using Blazor.Server.UI.Services.Navigation;
using Blazor.Server.UI.Services.Notifications;
using Blazor.Server.UI.Services.UserPreferences;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Constants.Localization;
using CleanArchitecture.Blazor.Infrastructure.Hubs;
using CleanArchitecture.Blazor.UI.Middlewares;
using Hangfire;
using Microsoft.AspNetCore.Http.Connections;
using MudBlazor.Services;
using MudExtensions.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Blazor.Server.UI;

public static class ConfigureServices
{
    public static IServiceCollection AddServerServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor(options =>
        {
            options.DetailedErrors = true;
            options.DisconnectedCircuitMaxRetained = 100;
            options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
            options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
            options.MaxBufferedUnacknowledgedRenderBatches = 10;
        }).AddHubOptions(options =>
        {
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.EnableDetailedErrors = false;
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.MaximumParallelInvocationsPerClient = 100;
            options.MaximumReceiveMessageSize = 64 * 1024;
            options.StreamBufferCapacity = 10;
        }).AddCircuitOptions(option => { option.DetailedErrors = true; });
        services.AddMudBlazorDialog();
        services.AddHotKeys2();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 4000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetExecutingAssembly());
            options.UseReduxDevTools();
        });

        services.AddScoped<LocalizationCookiesMiddleware>()
            .Configure<RequestLocalizationOptions>(options =>
            {
                options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.FallBackToParentUICultures = true;
            })
            .AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);

        services.AddMudExtensions();
        services.AddScoped<LayoutService>();
        services.AddBlazorDownloadFile();
        services.AddScoped<ExceptionHandlingMiddleware>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<INotificationService, InMemoryNotificationService>();
        services.AddHealthChecks();

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

    public static WebApplication ConfigureServer(this WebApplication app, IConfiguration config)
    {
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.MapHealthChecks("/health");
        app.UseExceptionHandler("/Error");
        app.MapFallbackToPage("/_Host");
        app.UseInfrastructure(config);
        app.UseMiddleware<LocalizationCookiesMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
            AsyncAuthorization = new[] { new HangfireDashboardAsyncAuthorizationFilter() }
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapHub<SignalRHub>(SignalR.HubUrl);
        });

        app.UseWebSockets();
        app.MapBlazorHub(options => options.Transports = HttpTransportType.WebSockets);

        return app;
    }
}