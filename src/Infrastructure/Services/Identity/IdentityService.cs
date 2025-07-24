// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService, IDisposable
{
    private readonly IFusionCache _fusionCache;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public IdentityService(
        IServiceScopeFactory scopeFactory,
        IFusionCache fusionCache,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _fusionCache = fusionCache;
        _mapper = mapper;
    }

    private TimeSpan RefreshInterval => TimeSpan.FromMinutes(60);

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetUserNameAsync:{userId}";
        var user = await _fusionCache.GetOrSetAsync(key,
             async _ =>
             {
                 using var scope = _scopeFactory.CreateScope();
                 var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                 return await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation);
             }, RefreshInterval);
        return user?.UserName;
    }

    public string GetUserName(string userId)
    {
        var key = $"GetUserName-byId:{userId}";
        var user = _fusionCache.GetOrSet(key, _ =>
        {
            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return userManager.Users.SingleOrDefault(u => u.Id == userId);
        }, RefreshInterval);
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException("User Not Found");
        return await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        var authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException("User Not Found");
        var principal = await userClaimsPrincipalFactory.CreateAsync(user);
        var result = await authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }

    public async Task<IDictionary<string, string?>> FetchUsers(string roleName,
        CancellationToken cancellation = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var result = await userManager.Users
            .Where(x => x.UserRoles.Any(y => y.Role.Name == roleName))
            .Include(x => x.UserRoles)
            .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }


    public async Task<ApplicationUserDto?> GetApplicationUserDto(string userName,
        CancellationToken cancellation = default)
    {
        var key = GetApplicationUserCacheKey(userName);
        var result = await _fusionCache.GetOrSetAsync(key,
            async _ =>
            {
                using var scope = _scopeFactory.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.Users
                    .Where(x => x.UserName == userName)
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellation);
            }, RefreshInterval);
        return result;
    }

    public async Task<List<ApplicationUserDto>?> GetUsers(string? tenantId, CancellationToken cancellation = default)
    {
        var key = $"GetApplicationUserDtoListWithTenantId:{tenantId}";
        Func<string?, CancellationToken, Task<List<ApplicationUserDto>?>> getUsersByTenantId =
            async (tenantId, token) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                if (string.IsNullOrEmpty(tenantId))
                {
                    return await userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync(token);
                }
                return await userManager.Users.Where(x => x.TenantId == tenantId).Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync(token);
            };
        var result = await _fusionCache.GetOrSetAsync(key, _=>getUsersByTenantId(tenantId, cancellation), RefreshInterval);
        return result;
    }

    public void RemoveApplicationUserCache(string userName)
    {
        _fusionCache.Remove(GetApplicationUserCacheKey(userName));
    }

    private string GetApplicationUserCacheKey(string userName)
    {
        return $"GetApplicationUserDto:{userName}";
    }

    public void Dispose()
    {
        // No long-lived resources to dispose
        GC.SuppressFinalize(this);
    }
}