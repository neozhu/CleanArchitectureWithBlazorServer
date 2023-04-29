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
    public HangfireDashboardAsyncAuthorizationFilter(string secretKey)
    {
        _secretKey = secretKey;
    }
    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var path = httpContext.Request.Path;
        var isResource = path != null && (path.Value.StartsWith("/css") || path.Value.StartsWith("/js") || path.Value.StartsWith("/font") || path.Value.StartsWith("/stats"));
        if (isResource) return true;
        var access_token = string.Empty;
        if (httpContext.Request.Query.ContainsKey(ACCESS_TOKEN))
        {
            access_token = httpContext.Request.Query[ACCESS_TOKEN].First();
        }
        if(string.IsNullOrEmpty(access_token))
            return false;

        var tokenheader = Encoding.UTF8.GetString(Convert.FromBase64String(access_token));
        return tokenheader.Equals("{\"alg\":\"HS256\",\"typ\":\"JWT\"}");


    }
}

 