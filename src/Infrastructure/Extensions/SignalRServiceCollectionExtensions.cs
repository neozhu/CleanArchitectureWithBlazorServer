using CleanArchitecture.Blazor.Infrastructure.Hubs;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class SignalRServiceCollectionExtensions
{
    public static void AddSignalRServices(this IServiceCollection services)
    {
        services.AddSingleton<IUsersStateContainer, UsersStateContainer>()
            .AddScoped<HubClient>()
            .AddSignalR();
    }
}