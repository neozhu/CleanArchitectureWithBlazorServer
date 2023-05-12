// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DashboardSettings>(configuration.GetSection(DashboardSettings.Key));
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Key));
        services.Configure<AppConfigurationSettings>(configuration.GetSection(AppConfigurationSettings.Key));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DashboardSettings>>().Value);
        services.AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        services.AddSingleton(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value);
        services.AddScoped<AuthenticationStateProvider, BlazorAuthStateProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("BlazorDashboardDb");
                options.EnableSensitiveDataLogging();
            });
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>((p, m) =>
             {
                 var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                 m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
             });
        }

       
        services.AddScoped<IDbContextFactory<ApplicationDbContext>, BlazorContextFactory<ApplicationDbContext>>();
        services.AddTransient<IApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        services.AddScoped<ApplicationDbContextInitializer>();


        services.AddLocalizationServices();
        services.AddServices()
            .AddHangfireService()
            .AddSerialization()
            .AddMessageServices(configuration)
            .AddSignalRServices();
        services.AddAuthenticationService(configuration);
        services.AddHttpClientService();
        services.AddControllers();
        return services;
    }
}