using System.Net.Http.Headers;
using System.Reflection;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Infrastructure.Constants.Localization;
using CleanArchitecture.Blazor.Server.Hubs;
using CleanArchitecture.Blazor.Server.Middlewares;
using CleanArchitecture.Blazor.Server.UI.Hubs;
using CleanArchitecture.Blazor.Server.UI.Pages.Identity.Authentication;
using CleanArchitecture.Blazor.Server.UI.Services;
using CleanArchitecture.Blazor.Server.UI.Services.Layout;
using CleanArchitecture.Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Server.UI.Services.Notifications;
using CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;
using Hangfire;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using MudExtensions.Services;
using Polly;
using QuestPDF.Infrastructure;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Server.UI;

public static class DependencyInjection
{
    public static IServiceCollection AddServerUI(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddMudBlazorDialog()
            .AddMudServices(mudServicesConfiguration =>
            {
                mudServicesConfiguration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                mudServicesConfiguration.SnackbarConfiguration.PreventDuplicates = false;
                mudServicesConfiguration.SnackbarConfiguration.NewestOnTop = true;
                mudServicesConfiguration.SnackbarConfiguration.ShowCloseIcon = true;
                mudServicesConfiguration.SnackbarConfiguration.VisibleStateDuration = 4000;
                mudServicesConfiguration.SnackbarConfiguration.HideTransitionDuration = 500;
                mudServicesConfiguration.SnackbarConfiguration.ShowTransitionDuration = 500;
                mudServicesConfiguration.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
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

        services.AddHttpContextAccessor();
        services.AddScoped<HubClient>();
        
        services.AddMudExtensions()
            .AddScoped<CookieProvider>()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddScoped<LayoutService>()
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
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
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
        QuestPDF.Settings.License = LicenseType.Community;

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

  

        return app;
    }
}