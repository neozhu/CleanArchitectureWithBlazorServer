// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly IFusionCache _fusionCache;
    private readonly IMapper _mapper;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        IServiceScopeFactory scopeFactory,
        IApplicationSettings appConfig,
        IFusionCache fusionCache,
        IMapper mapper,
        IStringLocalizer<IdentityService> localizer)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _userClaimsPrincipalFactory =
            scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
 
        _fusionCache = fusionCache;
        _mapper = mapper;
        _localizer = localizer;
    }

    private TimeSpan RefreshInterval => TimeSpan.FromMinutes(60);

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetUserNameAsync:{userId}";
        var user = await _fusionCache.GetOrSetAsync(key,
             _ => _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId), RefreshInterval);
        return user?.UserName;
    }

    public string GetUserName(string userId)
    {
        var key = $"GetUserName-byId:{userId}";
        var user = _fusionCache.GetOrSet(key, _ => _userManager.Users.SingleOrDefault(u => u.Id == userId), RefreshInterval);
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        var result = await _userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<IDictionary<string, string?>> FetchUsers(string roleName,
        CancellationToken cancellation = default)
    {
        var result = await _userManager.Users
            .Where(x => x.UserRoles.Any(y => y.Role.Name == roleName))
            .Include(x => x.UserRoles)
            .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }

    public async Task UpdateLiveStatus(string userId, bool isLive, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsLive != isLive);
        if (user is not null)
        {
            user.IsLive = isLive;
            var result = await _userManager.UpdateAsync(user);
        }
    }

    public async Task<ApplicationUserDto?> GetApplicationUserDto(string userName,
        CancellationToken cancellation = default)
    {
        var key = GetApplicationUserCacheKey(userName);
        var result = await _fusionCache.GetOrSetAsync(key,
            _ =>  _userManager.Users.Where(x => x.UserName == userName).Include(x => x.UserRoles)
                .ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation), RefreshInterval);
        return result;
    }

    public async Task<List<ApplicationUserDto>?> GetUsers(string? tenantId, CancellationToken cancellation = default)
    {
        var key = $"GetApplicationUserDtoListWithTenantId:{tenantId}";
        Func<string?, CancellationToken, Task<List<ApplicationUserDto>?>> getUsersByTenantId =
            async (tenantId, token) =>
            {
                if (string.IsNullOrEmpty(tenantId))
                    return await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                        .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync();
                return await _userManager.Users.Where(x => x.TenantId == tenantId).Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync();
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
}