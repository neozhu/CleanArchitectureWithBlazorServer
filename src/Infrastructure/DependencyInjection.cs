// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("BlazorDashboardDb");
                options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Transient);
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                 options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Transient);
            services.AddDatabaseDeveloperPageExceptionFilter();
        }


        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
        });
        services.Configure<DashbordSettings>(configuration.GetSection(DashbordSettings.SectionName));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DashbordSettings>>().Value);
        services.AddScoped<IDbContextFactory<ApplicationDbContext>,BlazorContextFactory<ApplicationDbContext>>();
        services.AddTransient<IApplicationDbContext>(provider => provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        services.AddScoped<IDomainEventService, DomainEventService>();

        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
       

        services.AddSingleton<ProfileService>();
        services.AddScoped<IdentityAuthenticationService>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<IdentityAuthenticationService>());
        services.AddScoped<ICurrentUserService, CurrentUserService>();
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
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            // Default Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

        });
        services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.Cookie.HttpOnly = true;
            options.SlidingExpiration = true;
        });

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
        services.AddScoped<ExceptionHandlingMiddleware>();
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
            options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
            options.FallBackToParentUICultures = true;

        });



        services.AddControllers();
        services.AddSingleton<CircuitHandler, CircuitHandlerService>();
        services.AddSignalR();

        return services;
    }



}
