// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Application.Common.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Application.Common.Constants.Database;
using CleanArchitecture.Blazor.Application.Common.Constants.User;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Blazor.Infrastructure.Services.Circuits;
using CleanArchitecture.Blazor.Infrastructure.Services.Gemini;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;

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
    private const string LOGIN_PATH = "/account/login";
    private const int DEFAULT_LOCKOUT_TIME_SPAN_MINUTES = 5;
    private const int MAX_FAILED_ACCESS_ATTEMPTS = 5;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddApplicationSettings(configuration)
            .AddDatabaseServices(configuration)
            .AddIdentityAndSecurity(configuration)
            .AddBusinessServices(configuration)
            .AddCachingServices()
            .AddNotificationServices(configuration)
            .AddSessionManagement();
    
    }

    #region Configuration and Settings
    private static IServiceCollection AddApplicationSettings(this IServiceCollection services,
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

        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<MinioOptions>>().Value);

        services.Configure<AISettings>(configuration.GetSection(AISettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<AISettings>>().Value)
            .AddSingleton<IAISettings>(s => s.GetRequiredService<IOptions<AISettings>>().Value);
        return services;
    }
    #endregion

    #region Database and Persistence
    private static IServiceCollection AddDatabaseServices(this IServiceCollection services,
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
            services.AddDbContextFactory<ApplicationDbContext>((p, m) =>
            {
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                m.UseExceptionProcessor(databaseSettings.DBProvider);
                m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            }, ServiceLifetime.Scoped);
        services.AddScoped<IApplicationDbContextFactory, ApplicationDbContextFactory>();

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

    private static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder builder, string dbProvider)
    {

        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                EntityFramework.Exceptions.PostgreSQL.ExceptionProcessorExtensions.UseExceptionProcessor(builder);
                return builder;

            case DbProviderKeys.SqlServer:
                EntityFramework.Exceptions.SqlServer.ExceptionProcessorExtensions.UseExceptionProcessor(builder);
                return builder;


            case DbProviderKeys.SqLite:
                EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions.UseExceptionProcessor(builder);
                return builder;

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }
    #endregion

    #region Business Services
    private static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPicklistService, PicklistService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITenantSwitchService, TenantSwitchService>();
            

        // Configure HttpClient for GeolocationService
        services.AddHttpClient<IGeolocationService, GeolocationService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("User-Agent", "CleanArchitectureBlazorServer/1.0");
        });

        // Configure SecurityAnalysisService with options
        services.Configure<SecurityAnalysisOptions>(configuration.GetSection(SecurityAnalysisOptions.SectionName));
        services.AddScoped<ISecurityAnalysisService, SecurityAnalysisService>();

        return services
            .AddScoped<IValidationService, ValidationService>()
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<IExcelService, ExcelService>()
            .AddScoped<IUploadService, MinioUploadService>()
            .AddScoped<IPDFService, PDFService>()
            .AddTransient<IDocumentOcrJob, DocumentOcrJob>();
    }
    #endregion

    #region Notification Services
    private static IServiceCollection AddNotificationServices(this IServiceCollection services,
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
    #endregion

    #region Identity and Security
    private static IServiceCollection AddIdentityAndSecurity(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddScoped<IUserStore<ApplicationUser>, MultiTenantUserStore>();
        services.AddScoped<UserManager<ApplicationUser>, MultiTenantUserManager>();
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddClaimsPrincipalFactory<MultiTenantUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();



        // Replace the default SignInManager with AuditSignInManager
        var signInManagerDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(SignInManager<ApplicationUser>));
        if (signInManagerDescriptor != null)
        {
            services.Remove(signInManagerDescriptor);
        }
        services.AddScoped<SignInManager<ApplicationUser>, AuditSignInManager<ApplicationUser>>();

        // Add the custom role validator MultiTenantRoleValidator to override the default validation logic.
        // Ensures role names are unique within each tenant.
        services.AddScoped<IRoleValidator<ApplicationRole>, MultiTenantRoleValidator>();

        // Find the default RoleValidator<ApplicationRole> registration in the service collection.
        var defaultRoleValidator = services.FirstOrDefault(descriptor => descriptor.ImplementationType == typeof(RoleValidator<ApplicationRole>));

        // If the default role validator is found, remove it to ensure only MultiTenantRoleValidator is used.
        if (defaultRoleValidator != null)
        {
            services.Remove(defaultRoleValidator);
        }
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
            .AddScoped<IUserProfileState, UserProfileState>()
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
            options.LoginPath = LOGIN_PATH;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();



        return services;
    }
    #endregion

    #region Caching Services
    private static IServiceCollection AddCachingServices(this IServiceCollection services)
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
            FactorySoftTimeout = TimeSpan.FromSeconds(10),
            FactoryHardTimeout = TimeSpan.FromSeconds(30),
            AllowTimedOutFactoryBackgroundCompletion = true,
        });
        return services;
    }
    #endregion

    #region Session Management
    private static IServiceCollection AddSessionManagement(this IServiceCollection services)
    {
        // User context management
        services.AddSingleton<IHubFilter, UserContextHubFilter>();
        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();
        services.AddSingleton<IUserContextLoader, UserContextLoader>();
        
        // Circuit and state management
        services.AddScoped<CircuitHandler, UserSessionCircuitHandler>();
        services.AddSingleton<IUsersStateContainer, UsersStateContainer>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
    #endregion

 
}
