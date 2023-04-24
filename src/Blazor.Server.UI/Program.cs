using Blazor.Server.UI;
using Blazor.Server.UI.Services.Notifications;
using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("Serilog", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Update", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.BackgroundJobServer", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.Server.BackgroundServerProcess", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.Processing.BackgroundExecution", LogEventLevel.Error)
              
          .Enrich.FromLogContext()
          .Enrich.WithClientIp()
          .Enrich.WithClientAgent()
          .WriteTo.Console()
    );

builder.Services.AddBlazorUiServices();
builder.Services.AddInfrastructureServices(builder.Configuration)
                .AddApplicationServices();

var app = builder.Build();

app.MapHealthChecks("/health");
app.UseExceptionHandler("/Error");
app.MapFallbackToPage("/_Host");
app.UseInfrastructure(builder.Configuration);
app.UseWebSockets();
app.MapBlazorHub(options=>options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets);

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
await app.RunAsync();