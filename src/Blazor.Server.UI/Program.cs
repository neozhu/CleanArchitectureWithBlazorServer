using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;
using Blazor.Server.UI;
using Blazor.Server.UI.Services.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("Serilog", LogEventLevel.Error)
                .MinimumLevel.Override("BlazorState.Store", LogEventLevel.Error)
                .MinimumLevel.Override("BlazorState.Subscriptions", LogEventLevel.Error)
                .MinimumLevel.Override("BlazorState.Pipeline.State.CloneStateBehavior", LogEventLevel.Error)
                .MinimumLevel.Override("BlazorState.Pipeline.RenderSubscriptions.RenderSubscriptionsPostProcessor", LogEventLevel.Error)
          .Enrich.FromLogContext()
          .Enrich.WithClientIp()
          .Enrich.WithClientAgent()
          .WriteTo.Console()
    );

builder.Services.AddBlazorUIServices();
builder.Services.AddInfrastructureServices(builder.Configuration)
                .AddApplicationServices();

var app = builder.Build();
app.MapBlazorHub();
app.MapHealthChecks("/health");
app.UseExceptionHandler("/Error");
app.MapFallbackToPage("/_Host");
app.UseInfrastructure(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
        var notificationService = scope.ServiceProvider.GetService<INotificationService>();
        if (notificationService is InMemoryNotificationService inmemoryService)
        {
            inmemoryService.Preload();
        }
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
await app.RunAsync();