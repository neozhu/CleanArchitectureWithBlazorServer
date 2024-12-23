using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Server.UI;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.RegisterSerilog();
    builder.WebHost.UseStaticWebAssets();

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddServerUI(builder.Configuration);
    var app = builder.Build();

    app.ConfigureServer(builder.Configuration);

    await app.InitializeDatabaseAsync().ConfigureAwait(false);

    await app.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    var m = ex.Message;
    throw;
}
