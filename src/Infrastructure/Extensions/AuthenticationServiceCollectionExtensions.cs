using System.Reflection;
using System.Text;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Permission;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(options =>
        {
            var identitySettings = configuration.GetRequiredSection(IdentitySettings.Key).Get<IdentitySettings>();
            // Password settings
            options.Password.RequireDigit = identitySettings!.RequireDigit;
            options.Password.RequiredLength = identitySettings.RequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.RequireUpperCase;
            options.Password.RequireLowercase = identitySettings.RequireLowerCase;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identitySettings.DefaultLockoutTimeSpan);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;

            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            // User settings
            options.User.RequireUniqueEmail = true;

        });
        services
                .AddScoped<IIdentityService, IdentityService>()
                .AddAuthorization(options =>
                {
                    options.AddPolicy("CanPurge", policy => policy.RequireUserName(UserName.Administrator));
                    // Here I stored necessary permissions/roles in a constant
                    foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                    {
                        var propertyValue = prop.GetValue(null);
                        if (propertyValue is not null)
                        {
                            options.AddPolicy((string)propertyValue, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, (string)propertyValue));
                        }
                    }
                })
                 .AddAuthentication()
                 .AddJwtBearer(options =>
                 {
                     options.SaveToken = true;
                     options.RequireHttpsMetadata = false;
                     options.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidateIssuerSigningKey = false,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yn4$#cr=+i@eljzlhhr2xlgf98aud&(3&!po3r60wlm^3*huh#")),
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         RoleClaimType = ClaimTypes.Role,
                         ClockSkew = TimeSpan.Zero,
                         ValidateLifetime = true
                     };

                     options.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = context =>
                         {
                             var accessToken = context.Request.Headers.Authorization;
                             var path = context.HttpContext.Request.Path;
                             if (!string.IsNullOrEmpty(accessToken) &&
                                 path.StartsWithSegments("/signalRHub"))
                             {
                                 context.Token = accessToken.ToString().Substring(7);
                             }
                             return Task.CompletedTask;
                         }
                     };
                 });


        services.AddSingleton<UserDataProvider>();
        services.AddSingleton<IUserDataProvider>(sp =>
        {
            var service = sp.GetRequiredService<UserDataProvider>();
            service.Initialize();
            return service;
        });
        return services;
    }

}
