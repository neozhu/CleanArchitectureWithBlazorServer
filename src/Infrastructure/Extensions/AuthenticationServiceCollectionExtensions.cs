using System.Reflection;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class AuthenticationServiceCollectionExtensions
{
    public static void AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
      => services
                 .AddScoped<IdentityAuthenticationService>()
                 .AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<IdentityAuthenticationService>())
                 .AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationClaimsIdentityFactory>()
                 .AddTransient<IIdentityService, IdentityService>()
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
                 .AddAuthentication().TryConfigureMicrosoftAccount(configuration)
                                     .TryConfigureGoogleAccount(configuration);

}
