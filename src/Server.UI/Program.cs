using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using CleanArchitecture.Blazor.Server.UI;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();
builder.WebHost.UseStaticWebAssets();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddServerUI(builder.Configuration);

builder.Services.AddMudServices();

var app = builder.Build();

app.ConfigureServer(builder.Configuration);

if (app.Environment.IsDevelopment())
    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitialiseAsync();
        await initializer.SeedAsync();
    }

await app.RunAsync();