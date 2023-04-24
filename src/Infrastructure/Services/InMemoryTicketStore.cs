using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
/// <summary>
/// Adapted from https://github.com/aspnet/Security/blob/dev/samples/CookieSessionSample/MemoryCacheTicketStore.cs
/// to manage large identity cookies.
/// More info: http://www.dotnettips.info/post/2581
/// And http://www.dotnettips.info/post/2575
/// </summary>
public class InMemoryTicketStore : ITicketStore
{
    private readonly IMemoryCache _cache;

    public InMemoryTicketStore(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache)); ;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);

        return Task.CompletedTask;
    }

    public Task<AuthenticationTicket> RetrieveAsync(string key)
    {
        _cache.TryGetValue(key, out AuthenticationTicket ticket);
        return Task.FromResult(ticket);
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var options = new MemoryCacheEntryOptions().SetSize(1);
        var expiresUtc = ticket.Properties.ExpiresUtc;
        if (expiresUtc.HasValue)
        {
            options.SetAbsoluteExpiration(expiresUtc.Value);
        }

        if (ticket.Properties.AllowRefresh ?? false)
        {
            options.SetSlidingExpiration(TimeSpan.FromMinutes(60));//TODO: configurable.
        }

        _cache.Set(key, ticket, options);

        return Task.FromResult(0);
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = ticket.Principal.Claims
          .First(c => c.Type == ClaimTypes.Name).Value;

        await RenewAsync(key, ticket);
        return key;
    }
}