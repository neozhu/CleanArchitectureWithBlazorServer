using Blazor.Analytics;
using Blazor.Server.UI.Services.Layout;
using Blazor.Server.UI.Services.Navigation;
using Blazor.Server.UI.Services.Notifications;
using Blazor.Server.UI.Services.UserPreferences;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using MudBlazor.Services;
using MudExtensions.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Blazor.Server.UI;

public static class ConfigureServices
{
    public static WebApplicationBuilder AddBlazorUiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor(
                options =>
                {
                    options.DetailedErrors = true;
                    options.DisconnectedCircuitMaxRetained = 100;
                    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
                    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
                    options.MaxBufferedUnacknowledgedRenderBatches = 10;
                }
            ).AddHubOptions(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                options.EnableDetailedErrors = false;
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.MaximumParallelInvocationsPerClient = 100;
                options.MaximumReceiveMessageSize = 64 * 1024;
                options.StreamBufferCapacity = 10;
            })
            .AddCircuitOptions(option => { option.DetailedErrors = true; });
        builder.Services.AddMudBlazorDialog();
        builder.Services.AddHotKeys2();
        builder.Services.AddMudServices(config =>
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

        builder.Services.AddMudExtensions();
        builder.Services.AddScoped<LayoutService>();
        builder.Services.AddBlazorDownloadFile();
        builder.Services.AddScoped<IUserPreferencesService, UserPreferencesService>();
        builder.Services.AddScoped<IMenuService, MenuService>();
        builder.Services.AddScoped<INotificationService, InMemoryNotificationService>();
        builder.Services.AddHealthChecks();

        var privacySettings = builder.Configuration.GetRequiredSection(PrivacySettings.Key).Get<PrivacySettings>();
        if (privacySettings is not { UseGoogleAnalytics: true }) return builder;

        if (privacySettings.GoogleAnalyticsKey is null or "")
            throw new ArgumentNullException(nameof(privacySettings.GoogleAnalyticsKey));

        builder.Services.AddGoogleAnalytics(privacySettings.GoogleAnalyticsKey);

        return builder;
    }
}