using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using CleanArchitecture.Blazor.Server;
using CleanArchitecture.Blazor.Server.UI;
using CleanArchitecture.Blazor.Server.UI.Services.Notifications;

try
{
    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
    {
        Formatting = Newtonsoft.Json.Formatting.Indented,
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    };
    var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();
builder.WebHost.UseStaticWebAssets();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddServer(builder.Configuration)
    .AddServerUI(builder.Configuration);

var app = builder.Build();

app.ConfigureServer(builder.Configuration);

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