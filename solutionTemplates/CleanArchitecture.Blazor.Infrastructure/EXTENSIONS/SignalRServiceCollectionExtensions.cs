using CleanArchitecture.Blazor.$safeprojectname$.Hubs;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.$safeprojectname$.Extensions;
public static class SignalRServiceCollectionExtensions
{
    public static void AddSignalRServices(this IServiceCollection services)
        => services.AddSingleton<IUsersStateContainer, UsersStateContainer>()
                   .AddScoped<CircuitHandler, CircuitHandlerService>()
                   .AddScoped<HubClient>()
                   .AddSignalR();
}
