using System.Net.Http.Headers;
using System.Reflection;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Infrastructure.Constants.Localization;
using CleanArchitecture.Blazor.Server.Common.Interfaces;
using CleanArchitecture.Blazor.Server.Hubs;
using CleanArchitecture.Blazor.Server.Middlewares;
using CleanArchitecture.Blazor.Server.UI.Hubs;
using CleanArchitecture.Blazor.Server.UI.Services;
using CleanArchitecture.Blazor.Server.UI.Services.Layout;
using CleanArchitecture.Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Server.UI.Services.Notifications;
using CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;
using Hangfire;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using MudExtensions.Services;
using Polly;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Server.UI;

public static class DependencyInjection
{
    public static IServiceCollection AddServerUI(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddCascadingAuthenticationState();
        services.AddMudBlazorDialog()
            .AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 4000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            }).AddHotKeys2();

        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetExecutingAssembly());
            options.UseReduxDevTools();
        });

        services.AddHttpClient("ocr", c =>
        {
            c.BaseAddress = new Uri("https://paddleocr.blazorserver.com/uploadocr");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
        }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(30)));

        services.AddScoped<HubClient>();

        services.AddMudExtensions()
            .AddScoped<AuthenticationStateProvider, BlazorAuthenticationStateProvider>()
            .AddScoped<LayoutService>()
            .AddBlazorDownloadFile()
            .AddScoped<IUserPreferencesService, UserPreferencesService>()
            .AddScoped<IMenuService, MenuService>()
            .AddScoped<INotificationService, InMemoryNotificationService>();

        return services;
    }

    public static WebApplication ConfigureServer(this WebApplication app, IConfiguration config)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.MapHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"Files")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"Files"));
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            RequestPath = new PathString("/Files")
        });

        var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(LocalizationConstants.SupportedLanguages.Select(x => x.Code).First())
                  .AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray())
                  .AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());

        app.UseRequestLocalization(localizationOptions);

        app.UseMiddleware<LocalizationCookiesMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
            AsyncAuthorization = new[] { new HangfireDashboardAsyncAuthorizationFilter() }
        });





        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.MapHub<ServerHub>(ISignalRHub.Url);

        return app;
    }
}