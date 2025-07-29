using CleanArchitecture.Blazor.Application.Common.FusionCache;
using Microsoft.Extensions.Hosting;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class HostExtensions
{
    public static void InitializeCacheFactory(this IHost host)
    {
        FusionCacheFactory.Configure(host.Services);
    }
    public static async Task InitializeDatabaseAsync(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
            await initializer.InitialiseAsync().ConfigureAwait(false);

            var env = host.Services.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                await initializer.SeedAsync().ConfigureAwait(false);
            }
        }
    }

   
}
