using CleanArchitecture.Blazor.Infrastructure.Constants.Localization;
using CleanArchitecture.Blazor.Infrastructure.Hubs;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration config)
    {

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseExceptionHandler("/Error");
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
        app.UseMiddlewares();
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

        return app;
    }


    
}
