using System.Net.Http.Headers;
using System.Reflection;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.Localization;
using CleanArchitecture.Blazor.Server.UI.Hubs;
using CleanArchitecture.Blazor.Server.UI.Services;
using CleanArchitecture.Blazor.Server.UI.Services.Fusion;
using CleanArchitecture.Blazor.Server.UI.Services.JsInterop;
using CleanArchitecture.Blazor.Server.UI.Services.Layout;
using CleanArchitecture.Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Server.UI.Services.Notifications;
using CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;
using Hangfire;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using Polly;
using QuestPDF;
using QuestPDF.Infrastructure;
using ActualLab.Fusion;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using ActualLab.Fusion.Extensions;
using CleanArchitecture.Blazor.Server.UI.Middlewares;


namespace CleanArchitecture.Blazor.Server.UI;

/// <summary>
/// Provides dependency injection configuration for the server UI.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds server UI services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">The configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddServerUI(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorComponents().AddInteractiveServerComponents().AddHubOptions(options=> options.MaximumReceiveMessageSize = 64 * 1024);
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddMudServices(config =>
        {
            MudGlobal.InputDefaults.ShrinkLabel = true;
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
           
            // we're currently planning on deprecating `PreventDuplicates`, at least to the end dev. however,
            // we may end up wanting to instead set it as internal because the docs project relies on it
            // to ensure that the Snackbar always allows duplicates. disabling the warning for now because
            // the project is set to treat warnings as errors.
#pragma warning disable 0618
            config.SnackbarConfiguration.PreventDuplicates = false;
#pragma warning restore 0618
        });
        services.AddMudPopoverService();
        services.AddMudBlazorSnackbar();
        services.AddMudBlazorDialog();
        services.AddHotKeys2();

        // Fusion services
        services.AddFusion(fusion =>
        {
            fusion.AddInMemoryKeyValueStore();
            fusion.AddService<IUserSessionTracker, UserSessionTracker>();
            fusion.AddService<IOnlineUserTracker, OnlineUserTracker>();
        });


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
            .AddSignalR(options=>options.MaximumReceiveMessageSize=64*1024);
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();


        services.AddHttpClient("ocr", c =>
        {
            c.BaseAddress = new Uri("http://10.33.1.150:8000/ocr/predict-by-file");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(30)));
        services.AddScoped<LocalTimezoneOffset>();
        services.AddHttpContextAccessor();
        services.AddScoped<HubClient>();
        services
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddScoped<LayoutService>()
            .AddScoped<DialogServiceHelper>()
            .AddScoped<PermissionHelper>()
            .AddBlazorDownloadFile()
            .AddScoped<IUserPreferencesService, UserPreferencesService>()
            .AddScoped<IMenuService, MenuService>()
            .AddScoped<InMemoryNotificationService>()
            .AddScoped<INotificationService>(sp =>
            {
                var service = sp.GetRequiredService<InMemoryNotificationService>();
                service.Preload();
                return service;
            });


        return services;
    }

    /// <summary>
    /// Configures the server pipeline.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <param name="config">The configuration.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication ConfigureServer(this WebApplication app, IConfiguration config)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithRedirects("/404");
        app.MapHealthChecks("/health");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"Files")))
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"Files"));

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            RequestPath = new PathString("/Files")
        });

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(LocalizationConstants.SupportedLanguages.Select(x => x.Code).First())
            .AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray())
            .AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
        app.UseRequestLocalization(localizationOptions);
        app.UseMiddleware<LocalizationCookiesMiddleware>();
        app.UseExceptionHandler();
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
            AsyncAuthorization = new[] { new HangfireDashboardAsyncAuthorizationFilter() }
        });
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.MapHub<ServerHub>(ISignalRHub.Url);

        //QuestPDF License configuration
        Settings.License = LicenseType.Community;

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();
        app.UseForwardedHeaders();
        app.UseWebSockets(new WebSocketOptions()
        { // We obviously need this
            KeepAliveInterval = TimeSpan.FromSeconds(30), // Just in case
        });
        return app;
    }
}
