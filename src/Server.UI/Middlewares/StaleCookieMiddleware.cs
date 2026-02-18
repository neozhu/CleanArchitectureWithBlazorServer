using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Blazor.Server.UI.Middlewares;

public class StaleCookieMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDataProtector _protector;
    private readonly ILogger<StaleCookieMiddleware> _logger;
    private readonly StaleCookieOptions _options;

    private const string CanaryPayload = "ok";

    public StaleCookieMiddleware(
        RequestDelegate next,
        IDataProtectionProvider dataProtection,
        ILogger<StaleCookieMiddleware> logger,
        IOptions<StaleCookieOptions> options) // Inject configuration
    {
        _next = next;
        _protector = dataProtection.CreateProtector("StaleCookieMiddleware.Canary");
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Optimization: Skip decryption overhead for static files or other requests that don't need to be checked.
        // Note: This depends on your routing configuration. Sometimes static files don't go through Endpoint routing, this is just an example.
        if (_options.IgnoreStaticFiles && IsStaticFile(context))
        {
            await _next(context);
            return;
        }

        var cookieName = _options.CanaryCookieName;
        var hasCanary = context.Request.Cookies.TryGetValue(cookieName, out var canaryValue);

        bool keysAreStale = false;
        bool shouldSetCanary = true; // By default, needs to be set/refreshed

        if (hasCanary && !string.IsNullOrEmpty(canaryValue))
        {
            try
            {
                var result = _protector.Unprotect(canaryValue);
                if (result == CanaryPayload)
                {
                    // Verification successful: keys are not expired
                    keysAreStale = false;
                    // Optimization: If the cookie is valid, no need to resend Set-Cookie on every response, reduce header size.
                    // Unless you want to implement Sliding Expiration, set this to false.
                    shouldSetCanary = false;
                }
                else
                {
                    keysAreStale = true;
                }
            }
            catch (CryptographicException)
            {
                // Decryption failed, indicating the key has changed
                keysAreStale = true;
            }
        }

        if (keysAreStale)
        {
            HandleStaleCookies(context);
            // Since the old one is invalid, we need to set a new one in the response
            shouldSetCanary = true;
        }

        // Continue executing the pipeline
        await _next(context);

        // Response phase: Write new canary cookie
        if (shouldSetCanary && !context.Response.HasStarted && context.Response.StatusCode < 400)
        {
            SetCanaryCookie(context);
        }
    }

    private void HandleStaleCookies(HttpContext context)
    {
        var canaryName = _options.CanaryCookieName;
        var staleCookies = context.Request.Cookies.Keys
            .Where(k => IsEncryptedCookie(k) && !string.Equals(k, canaryName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (staleCookies.Count > 0)
        {
            _logger.LogWarning(
                "Detected Key Rotation. Clearing {Count} stale cookie(s): {Cookies}",
                staleCookies.Count,
                string.Join(", ", staleCookies));

            var staleSet = new HashSet<string>(staleCookies, StringComparer.OrdinalIgnoreCase);

            // 1. Notify the browser to delete
            foreach (var cookie in staleCookies)
            {
                context.Response.Cookies.Delete(cookie);
            }

            // 2. Most importantly: Remove from current request to prevent errors in subsequent middleware
            // Rebuild the Cookie Header
            var freshCookies = context.Request.Cookies
                .Where(c => !staleSet.Contains(c.Key))
                .Select(c => $"{Uri.EscapeDataString(c.Key)}={Uri.EscapeDataString(c.Value)}");

            context.Request.Headers.Cookie = string.Join("; ", freshCookies);
        }
    }

    private void SetCanaryCookie(HttpContext context)
    {
        try
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Recommendation: Force Secure in production environment
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                MaxAge = TimeSpan.FromDays(365) // Long-term validity
            };

            // Protect the payload
            var protectedPayload = _protector.Protect(CanaryPayload);
            context.Response.Cookies.Append(_options.CanaryCookieName, protectedPayload, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set canary cookie.");
        }
    }

    private bool IsEncryptedCookie(string name)
    {
        // Use prefixes from configuration, no longer hardcoded
        return _options.EncryptedCookiePrefixes.Any(prefix =>
            name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    // Simple static file judgment logic (optional)
    private bool IsStaticFile(HttpContext context)
    {
        // Better approach: Check Endpoint Metadata or simple extension check
        var path = context.Request.Path.Value;
        if (string.IsNullOrEmpty(path)) return false;

        // Simple example
        return path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".wasm", StringComparison.OrdinalIgnoreCase);
    }
}
public class StaleCookieOptions
{
    /// <summary>
    /// List of encrypted cookie prefixes to monitor and remove.
    /// </summary>
    public List<string> EncryptedCookiePrefixes { get; set; } = new()
    {
        ".AspNetCore.Antiforgery",
        ".AspNetCore.Identity",
        ".AspNetCore.Cookies",
        ".AspNetCore.Session"
    };

    /// <summary>
    /// Name of the canary cookie
    /// </summary>
    public string CanaryCookieName { get; set; } = ".AspNetCore.DPCheck";

    /// <summary>
    /// Whether to ignore checks on static files (determined by Endpoint)
    /// </summary>
    public bool IgnoreStaticFiles { get; set; } = true;
}