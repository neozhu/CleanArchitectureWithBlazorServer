// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
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
                 m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
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
            .AddMessageServices(configuration);

        services.AddAuthenticationService(configuration);
        services.AddSimpleJwtService(options =>
        {
            options.UseCookie = false;

            options.AccessSigningOptions = new JwtSigningOptions()
            {
                SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yn4$#cr=+i@eljzlhhr2xlgf98aud&(3&!po3r60wlm^3*huh#")),
                Algorithm = SecurityAlgorithms.HmacSha256,
                ExpirationMinutes = 120,
            };

            options.RefreshSigningOptions = new JwtSigningOptions()
            {
                SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("e_qmg*)=vr9yxpp^g^#((wkwk7fh#+3qy!zzq+r-hifw2(_u+=")),
                Algorithm = SecurityAlgorithms.HmacSha256,
                ExpirationMinutes = 2880,
            };
            options.AccessValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = options.AccessSigningOptions.SigningKey,
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
            };
            options.RefreshValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = options.AccessSigningOptions.SigningKey,
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
            };
        });
        services.AddScoped<AuthenticationStateProvider, BlazorAuthStateProvider>();
        services.AddSignalRServices();
        services.AddHttpClientService();
        services.AddControllers();
        return services;
    }
}