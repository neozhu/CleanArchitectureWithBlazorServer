using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
/// <summary>
/// Extensions for configuring login via third party authentication providers.
/// </summary>
public static class AuthenticationProvidersServiceCollectionExtensions
{
    /// <summary>
    /// Try to configure Microsoft account login if the application configuration has ClientId and ClientSecret.
    /// </summary>
    /// <param name="authenticationBuilder">Authentication Builder.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>Authentication builder configured to sign in with a Microsoft account,
    /// if the required keys are specified in the application configuration.</returns>
    public static AuthenticationBuilder TryConfigureMicrosoftAccount(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
    {
        var microsoftAccountClientId = configuration["Authentication:Microsoft:ClientId"];
        var microsoftAccountClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
        if (string.IsNullOrWhiteSpace(microsoftAccountClientId) || string.IsNullOrWhiteSpace(microsoftAccountClientSecret))
            return authenticationBuilder;

        return authenticationBuilder.AddMicrosoftAccount(options =>
        {
            options.ClientId = microsoftAccountClientId;
            options.ClientSecret = microsoftAccountClientSecret;
            options.AccessDeniedPath = "/Login";
        });
    }

    /// <summary>
    /// Try to configure Google account login if the application configuration has ClientId and ClientSecret.
    /// </summary>
    /// <param name="authenticationBuilder">Authentication Builder.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>Authentication builder configured for Google account login,
    /// if the required keys are specified in the application configuration.</returns>
    public static AuthenticationBuilder TryConfigureGoogleAccount(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
    {
        var googleAccountClientId = configuration["Authentication:Google:ClientId"];
        var googleAccountClientSecret = configuration["Authentication:Google:ClientSecret"];
        if (string.IsNullOrWhiteSpace(googleAccountClientId) || string.IsNullOrWhiteSpace(googleAccountClientSecret))
            return authenticationBuilder;

        return authenticationBuilder.AddGoogle(options =>
        {
            options.ClientId = googleAccountClientId;
            options.ClientSecret = googleAccountClientSecret;
            options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
            {
                OnCreatingTicket = c =>
                {
                    var identity = (ClaimsIdentity)c.Principal.Identity;
                    var avatar = c.User.GetProperty("picture").GetString();
                    identity.AddClaim(new Claim("avatar", avatar));
                    return Task.CompletedTask;
                }
            };
        });
    }
}