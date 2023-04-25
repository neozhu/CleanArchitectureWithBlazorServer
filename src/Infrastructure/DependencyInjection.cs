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
        else if (configuration.GetValue<bool>("UsePostgresSQL"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Enabling legacy datetime support for PostgresSQL
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                options.UseNpgsql(
                    configuration.GetConnectionString("PostgresSQLConnection"),
                    builder =>
                    {
                        builder.MigrationsAssembly("Blazor.Server.UI");
                        builder.EnableRetryOnFailure(5);
                        builder.CommandTimeout(15);
                    });
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });
            services.AddDatabaseDeveloperPageExceptionFilter();
        }
        else if (configuration.GetValue<bool>("UseMSSQLServer"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("MSSQLServerConnection"),
                    builder =>
                    {
                        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        builder.CommandTimeout(15);
                    });
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });
            services.AddDatabaseDeveloperPageExceptionFilter();
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("BlazorDashboardDb");
                options.EnableSensitiveDataLogging();
            });
        }

        services.Configure<DashboardSettings>(configuration.GetSection(DashboardSettings.SectionName));
        services.Configure<AppConfigurationSettings>(configuration.GetSection(AppConfigurationSettings.SectionName));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DashboardSettings>>().Value);
        services.AddSingleton(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value);
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