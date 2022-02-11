// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using FluentValidation.AspNetCore;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

namespace CleanArchitecture.Blazor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("CleanArchitecture.BlazorDb")
                );

         
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))

                );
            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        services.Configure<DashbordSettings>(configuration.GetSection(DashbordSettings.SectionName));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DashbordSettings>>().Value);
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        services.AddScoped<IDomainEventService, DomainEventService>();


        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IExcelService, ExcelService>();
        services.AddTransient<IUploadService, UploadService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.Configure<AppConfigurationSettings>(configuration.GetSection("AppConfigurationSettings"));
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.AddTransient<IMailService, SMTPMailService>();
        services.AddTransient<IDictionaryService, DictionaryService>();
        services.AddAuthentication();
        services.Configure<IdentityOptions>(options =>
        {
            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            // Default Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        });
        services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));
        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                }
            }
        });
        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationClaimsIdentityFactory>();
        // Localization
        services.AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);
        services.AddScoped<LocalizationCookiesMiddleware>();
        services.AddScoped<ExceptionMiddleware>();
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
            options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
            options.FallBackToParentUICultures = true;

        });
        services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
        services.AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp => {
            // this is safe because 
            //     the `RevalidatingIdentityAuthenticationStateProvider` extends the `ServerAuthenticationStateProvider`
            var provider = (ServerAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>();
            return provider;
        });

        services.AddControllers();
        services.AddSignalR();

        return services;
    }


 
}
