using System.Net;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static string? GetClientIp(this HttpContext? httpContext)
    {
        try
        {
            if (httpContext == null)
                return null;

            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var firstIpRaw = forwardedFor.Split(',').FirstOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(firstIpRaw))
                {
                    var cleanedForward = NormalizeForwardedIp(firstIpRaw);
                    if (!string.IsNullOrEmpty(cleanedForward))
                        return cleanedForward;
                }
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                var cleanedReal = NormalizeForwardedIp(realIp);
                if (!string.IsNullOrEmpty(cleanedReal))
                    return cleanedReal;
            }

            var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
            if (remoteIpAddress != null && remoteIpAddress.IsIPv4MappedToIPv6)
            {
                remoteIpAddress = remoteIpAddress.MapToIPv4();
            }
            var remoteIp = remoteIpAddress?.ToString();

            if (remoteIp == "::1" || remoteIp == "127.0.0.1")
            {
                return "127.0.0.1";
            }

            return SanitizeInput(remoteIp);
        }
        catch
        {
            return null;
        }
    }

    public static string? GetUserAgent(this HttpContext? httpContext)
    {
        if (httpContext == null)
            return null;

        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
        if (string.IsNullOrEmpty(userAgent))
            return null;

        return userAgent.Length > 1000 ? userAgent.Substring(0, 1000) : userAgent;
    }

    private static string? NormalizeForwardedIp(string raw)
    {
        raw = raw.Trim();
        if (raw.Count(c => c == ':') == 1 && !raw.Contains("::"))
        {
            var parts = raw.Split(':');
            if (IPAddress.TryParse(parts[0], out _))
            {
                raw = parts[0];
            }
        }
        if (raw.StartsWith("::ffff:"))
        {
            var ipv4Part = raw.Substring("::ffff:".Length);
            if (IPAddress.TryParse(ipv4Part, out _))
                raw = ipv4Part;
        }
        return SanitizeInput(raw);
    }

    private static string SanitizeInput(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        return input.Replace("\r", "").Replace("\n", "").Trim();
    }
}
