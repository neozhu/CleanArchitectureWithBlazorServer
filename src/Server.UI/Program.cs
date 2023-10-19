using CleanArchitecture.Blazor.Server.UI;
using CleanArchitecture.Blazor.Server.UI.Services.Notifications;
using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddServerServices(builder.Configuration);

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

await app.RunAsync();