using Blazor.Server.UI;
using Blazor.Server.UI.Services.Notifications;
using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.Connections;
using Newtonsoft.Json;

try
{
    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
    {
        Formatting = Newtonsoft.Json.Formatting.Indented,
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    };
    var builder = WebApplication.CreateBuilder(args);

    builder.RegisterSerilog();
    builder.AddBlazorUiServices();
    builder.Services.AddInfrastructureServices(builder.Configuration)
        .AddApplicationServices();

    var app = builder.Build();

    app.MapHealthChecks("/health");
    app.UseExceptionHandler("/Error");
    app.MapFallbackToPage("/_Host");
    app.UseInfrastructure(builder.Configuration);
    app.UseWebSockets();
    app.MapBlazorHub(options => options.Transports = HttpTransportType.WebSockets);

    if (app.Environment.IsDevelopment())
    {
        // Initialise and seed database
        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
            await initializer.InitialiseAsync();
            await initializer.SeedAsync();
            var notificationService = scope.ServiceProvider.GetService<INotificationService>();
            if (notificationService is InMemoryNotificationService inMemoryNotificationService)
            {
                inMemoryNotificationService.Preload();
            }
        }
    }
    else
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    using (var scope = app.Services.CreateScope())//for loading static data for first time
    {//todo alternative can be covered like cache or some more
        var initializer = scope.ServiceProvider.GetRequiredService<StaticData>();
        await initializer.LoadUserBaseRoles();
    }

    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}