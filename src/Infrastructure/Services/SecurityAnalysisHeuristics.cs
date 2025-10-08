// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Text.RegularExpressions;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public static class SecurityAnalysisHeuristics
{
    public static string NormalizeIpForThrottle(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return string.Empty;
        if (IPAddress.TryParse(ip, out var addr))
        {
            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return addr.ToString();
            }
            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                var bytes = addr.GetAddressBytes();
                return string.Join(':', Enumerable.Range(0, 8/2).Select(i => bytes[2*i].ToString("x2") + bytes[2*i+1].ToString("x2")));
            }
        }
        return ip.Trim();
    }

    public static string NormalizeIpForHeuristic(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return string.Empty;
        if (IPAddress.TryParse(ip, out var addr))
        {
            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var parts = ip.Split('.');
                if (parts.Length == 4) return $"{parts[0]}.{parts[1]}.{parts[2]}.*";
            }
            else if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                var segments = ip.Split(':');
                if (segments.Length >= 4)
                {
                    return string.Join(':', segments.Take(4)) + ":*";
                }
            }
        }
        return ip.Trim();
    }

    public static string ExtractUserAgentCore(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent)) return string.Empty;
        var match = Regex.Match(userAgent, "(Chrome|Firefox|Safari|Edg|Edge|OPR|Opera|Brave)[/ ](?<ver>\\d+)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var name = match.Groups[1].Value;
            var ver = match.Groups["ver"].Value;
            return $"{name}/{ver}";
        }
        var token = userAgent.Split(' ').FirstOrDefault(t => t.Contains('/'));
        if (token != null)
        {
            var major = token.Split('/');
            if (major.Length == 2)
            {
                var digits = new string(major[1].TakeWhile(char.IsDigit).ToArray());
                if (!string.IsNullOrEmpty(digits)) return $"{major[0]}/{digits}";
            }
        }
        return userAgent.Length > 40 ? userAgent.Substring(0, 40) : userAgent;
    }

    public static string ExtractRegionHierarchy(string? regionRaw, out string? display)
    {
        display = regionRaw;
        if (string.IsNullOrWhiteSpace(regionRaw)) return string.Empty;
        var separators = new[] { '|', '-', ',', '/' };
        var parts = regionRaw.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0) return regionRaw;
        if (parts.Length == 1)
        {
            display = parts[0];
            return parts[0];
        }
        display = string.Join("/", parts.Take(Math.Min(3, parts.Length)));
        return string.Join("/", parts.Take(2));
    }
}
