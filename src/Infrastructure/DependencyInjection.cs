// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MediatorWrapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Database;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Blazor.Infrastructure.Services.MediatorWrapper;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;
using CleanArchitecture.Blazor.Infrastructure.Services.Serialization;
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure;
public static class DependencyInjection
{
    private const string IDENTITY_SETTINGS_KEY = "IdentitySettings";
    private const string APP_CONFIGURATION_SETTINGS_KEY = "AppConfigurationSettings";
    private const string DATABASE_SETTINGS_KEY = "DatabaseSettings";
    private const string SMTP_CLIENT_OPTIONS_KEY = "SmtpClientOptions";
    private const string USE_IN_MEMORY_DATABASE_KEY = "UseInMemoryDatabase";
    private const string IN_MEMORY_DATABASE_NAME = "BlazorDashboardDb";
    private const string NPGSQL_ENABLE_LEGACY_TIMESTAMP_BEHAVIOR = "Npgsql.EnableLegacyTimestampBehavior";
    private const string POSTGRESQL_MIGRATIONS_ASSEMBLY = "CleanArchitecture.Blazor.Migrators.PostgreSQL";
    private const string MSSQL_MIGRATIONS_ASSEMBLY = "CleanArchitecture.Blazor.Migrators.MSSQL";
    private const string SQLITE_MIGRATIONS_ASSEMBLY = "CleanArchitecture.Blazor.Migrators.SqLite";
    private const string SMTP_CLIENT_OPTIONS_DEFAULT_FROM_EMAIL = "SmtpClientOptions:DefaultFromEmail";
    private const string EMAIL_TEMPLATES_PATH = "Resources/EmailTemplates";
    private const string DEFAULT_FROM_EMAIL = "noreply@blazorserver.com";
    private const string LOGIN_PATH = "/pages/authentication/login";
    private const int DEFAULT_LOCKOUT_TIME_SPAN_MINUTES = 5;
    private const int MAX_FAILED_ACCESS_ATTEMPTS = 5;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettings(configuration)
            .AddDatabase(configuration)
            .AddServices()
            .AddMessageServices(configuration);

        services
            .AddAuthenticationService(configuration)
            .AddFusionCacheService();

        services.AddSingleton<IUsersStateContainer, UsersStateContainer>();
        services.AddScoped<IScopedMediator, ScopedMediator>();
        return services;
    }

    private static IServiceCollection AddSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<IdentitySettings>(configuration.GetSection(IDENTITY_SETTINGS_KEY))
            .AddSingleton(s => s.GetRequiredService<IOptions<IdentitySettings>>().Value)
            .AddSingleton<IIdentitySettings>(s => s.GetRequiredService<IOptions<IdentitySettings>>().Value);

        services.Configure<AppConfigurationSettings>(configuration.GetSection(APP_CONFIGURATION_SETTINGS_KEY))
            .AddSingleton(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value)
            .AddSingleton<IApplicationSettings>(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value);

        services.Configure<DatabaseSettings>(configuration.GetSection(DATABASE_SETTINGS_KEY))
            .AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>()
            .AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        if (configuration.GetValue<bool>(USE_IN_MEMORY_DATABASE_KEY))
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(IN_MEMORY_DATABASE_NAME);
                options.EnableSensitiveDataLogging();
            });
        else
            services.AddDbContext<ApplicationDbContext>((p, m) =>
            {
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            });

        services.AddScoped<IDbContextFactory<ApplicationDbContext>, BlazorContextFactory<ApplicationDbContext>>();
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        services.AddScoped<ApplicationDbContextInitializer>();

        return services;
    }

    private static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider,
        string connectionString)
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                AppContext.SetSwitch(NPGSQL_ENABLE_LEGACY_TIMESTAMP_BEHAVIOR, true);
                return builder.UseNpgsql(connectionString,
                        e => e.MigrationsAssembly(POSTGRESQL_MIGRATIONS_ASSEMBLY))
                    .UseSnakeCaseNamingConvention();

            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(connectionString,
                    e => e.MigrationsAssembly(MSSQL_MIGRATIONS_ASSEMBLY));

            case DbProviderKeys.SqLite:
                return builder.UseSqlite(connectionString,
                    e => e.MigrationsAssembly(SQLITE_MIGRATIONS_ASSEMBLY));

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<PicklistService>()
            .AddSingleton<IPicklistService>(sp =>
            {
                var service = sp.GetRequiredService<PicklistService>();
                service.Initialize();
                return service;
            });

        services.AddSingleton<TenantService>()
            .AddSingleton<ITenantService>(sp =>
            {
                var service = sp.GetRequiredService<TenantService>();
                service.Initialize();
                return service;
            });

        return services.AddSingleton<ISerializer, SystemTextJsonSerializer>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<IValidationService, ValidationService>()
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<IExcelService, ExcelService>()
            .AddScoped<IUploadService, UploadService>()
            .AddScoped<IPDFService, PDFService>()
            .AddTransient<IDocumentOcrJob, DocumentOcrJob>();
    }

    private static IServiceCollection AddMessageServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var smtpClientOptions = new SmtpClientOptions();
        configuration.GetSection(SMTP_CLIENT_OPTIONS_KEY).Bind(smtpClientOptions);
        services.Configure<SmtpClientOptions>(configuration.GetSection(SMTP_CLIENT_OPTIONS_KEY));

        services.AddSingleton(smtpClientOptions);
        services.AddScoped<IMailService, MailService>();

        // configure your sender and template choices with dependency injection.
        var defaultFromEmail = configuration.GetValue<string>(SMTP_CLIENT_OPTIONS_DEFAULT_FROM_EMAIL);
        services.AddFluentEmail(defaultFromEmail ?? DEFAULT_FROM_EMAIL)
            .AddRazorRenderer(Path.Combine(Directory.GetCurrentDirectory(), EMAIL_TEMPLATES_PATH))
            .AddMailKitSender(smtpClientOptions);

        return services;
    }

    private static IServiceCollection AddAuthenticationService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(options =>
        {
            var identitySettings = configuration.GetRequiredSection(IDENTITY_SETTINGS_KEY).Get<IdentitySettings>();
            identitySettings = identitySettings ?? new IdentitySettings();
            // Password settings
            options.Password.RequireDigit = identitySettings.RequireDigit;
            options.Password.RequiredLength = identitySettings.RequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.RequireUpperCase;
            options.Password.RequireLowercase = identitySettings.RequireLowerCase;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(DEFAULT_LOCKOUT_TIME_SPAN_MINUTES);
            options.Lockout.MaxFailedAccessAttempts = MAX_FAILED_ACCESS_ATTEMPTS;
            options.Lockout.AllowedForNewUsers = true;

            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            //options.Tokens.EmailConfirmationTokenProvider = "Email";
            
        });

        services.AddScoped<IIdentityService, IdentityService>()
            .AddAuthorizationCore(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireUserName(UserName.Administrator));
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                             c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                        options.AddPolicy((string)propertyValue,
                            policy => policy.RequireClaim(ApplicationClaimTypes.Permission, (string)propertyValue));
                }
            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration.GetValue<string>("Authentication:Microsoft:ClientId") ?? string.Empty;
                microsoftOptions.ClientSecret = configuration.GetValue<string>("Authentication:Microsoft:ClientSecret") ?? string.Empty;
                //microsoftOptions.CallbackPath = new PathString("/pages/authentication/ExternalLogin"); # dotn't set this parameter!!
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration.GetValue<string>("Authentication:Google:ClientId") ?? string.Empty;
                googleOptions.ClientSecret = configuration.GetValue<string>("Authentication:Google:ClientSecret") ?? string.Empty; ;
            }
            )
            //.AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = configuration.GetValue<string>("Authentication:Facebook:AppId") ?? string.Empty;
            //    facebookOptions.AppSecret = configuration.GetValue<string>("Authentication:Facebook:AppSecret") ?? string.Empty;
            //})
            .AddIdentityCookies(options => { });

        services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromDays(15);
            options.SlidingExpiration = true;
            options.SessionStore = new MemoryCacheTicketStore();
        });
        services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();

        services.AddSingleton<UserService>()
            .AddSingleton<IUserService>(sp =>
            {
                var service = sp.GetRequiredService<UserService>();
                service.Initialize();
                return service;
            });

        return services;
    }

    private static IServiceCollection AddFusionCacheService(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddFusionCache().WithDefaultEntryOptions(new FusionCacheEntryOptions
        {
            // CACHE DURATION
            Duration = TimeSpan.FromMinutes(120),
            // FAIL-SAFE OPTIONS
            IsFailSafeEnabled = true,
            FailSafeMaxDuration = TimeSpan.FromHours(8),
            FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
            // FACTORY TIMEOUTS
            FactorySoftTimeout = TimeSpan.FromMilliseconds(1500),
            FactoryHardTimeout = TimeSpan.FromMilliseconds(3000),
            AllowTimedOutFactoryBackgroundCompletion = true,    
        });
        return services;
    }
}
