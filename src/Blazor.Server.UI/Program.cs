using Blazor.Server.UI;
using Blazor.Server.UI.Services.Notifications;
using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.Connections;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();
builder.Services.AddBlazorUiServices();
builder.Services.AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

WebApplication app = builder.Build();

app.MapHealthChecks("/health");
app.UseExceptionHandler("/Error");
app.MapFallbackToPage("/_Host");
app.UseInfrastructure(builder.Configuration);
app.UseWebSockets();
app.MapBlazorHub(options => options.Transports = HttpTransportType.WebSockets);

if (app.Environment.IsDevelopment())
{
    // Initialise and seed database
    using (IServiceScope scope = app.Services.CreateScope())
    {
        ApplicationDbContextInitializer initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitialiseAsync();
        await initializer.SeedAsync();
        INotificationService? notificationService = scope.ServiceProvider.GetService<INotificationService>();
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

await app.RunAsync();