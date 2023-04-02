using System.Reflection;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
       
        services
            .AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = false;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;

            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            // User settings
            options.User.RequireUniqueEmail = true;

        });
        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationClaimsIdentityFactory>()
                .AddScoped<IIdentityService, IdentityService>()
                .AddAuthorization(options =>
                 {
                     options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
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
                 .AddAuthentication();
   
        services.AddScoped<TokenAuthProvider>();
        services.AddScoped<UserDataProvider>();
        services.AddScoped<IUserDataProvider>(sp =>
        {
            var service = sp.GetRequiredService<UserDataProvider>();
            service.Initialize();
            return service;
        });
        return services;
    }

}
