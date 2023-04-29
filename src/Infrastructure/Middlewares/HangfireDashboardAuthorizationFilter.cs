using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Middlewares;
public class HangfireDashboardAsyncAuthorizationFilter : IDashboardAsyncAuthorizationFilter
{
    private readonly string _secretKey;
    const string ACCESS_TOKEN = "access_token";
    const string ISAUTHENTICATED = "IsAuthenticated";
    public HangfireDashboardAsyncAuthorizationFilter(string secretKey)
    {
        _secretKey = secretKey;
    }
    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var access_token = string.Empty;
        if (httpContext.Request.Query.ContainsKey(ACCESS_TOKEN))
        {
            access_token = httpContext.Request.Query[ACCESS_TOKEN].First();
        }
        else
        {
            var isAuthenticated = httpContext.Request.Cookies[ISAUTHENTICATED];
            return isAuthenticated?.Equals("true")??false;
        }
        if(string.IsNullOrEmpty(access_token))
            return false;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var result = await tokenHandler.ValidateTokenAsync(access_token, tokenValidationParameters);
        if (result.IsValid)
        {
            httpContext.Response.Cookies.Append(ISAUTHENTICATED, "true", new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(60)
            });
        }
        else
        {
            httpContext.Response.Cookies.Append(ISAUTHENTICATED, "false", new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(60)
            });
        }
        return true;
    }
}

 