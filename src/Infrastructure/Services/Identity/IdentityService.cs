// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly CustomUserManager _userManager;
    private readonly CustomRoleManager _roleManager;
    private readonly AppConfigurationSettings _appConfig;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAppCache _cache;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly IMapper _mapper;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        IServiceScopeFactory scopeFactory,
        AppConfigurationSettings appConfig,
        IAppCache cache,
        IMapper mapper,
        IStringLocalizer<IdentityService> localizer)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
        _roleManager = scope.ServiceProvider.GetRequiredService<CustomRoleManager>();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        _cache = cache;
        _mapper = mapper;
        _localizer = localizer;
    }

    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);

    private LazyCacheEntryOptions Options =>
        new LazyCacheEntryOptions().SetAbsoluteExpiration(RefreshInterval, ExpirationMode.LazyExpiration);

    public async Task<List<ApplicationRoleDto>> GetAllRoles()
    {
        var key = $"GetAllRoles";
        var roles = await _cache.GetOrAddAsync(key, async () => await _roleManager.Roles.ProjectTo<ApplicationRoleDto>(_mapper.ConfigurationProvider).ToListAsync());
        return roles;
    }
    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetUserNameAsync:{userId}";
        var user = await _cache.GetOrAddAsync(key,
            async () => await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId), Options);
        return user?.UserName;
    }

    public string GetUserName(string userId)
    {
        var key = $"GetUserName-byId:{userId}";
        var user = _cache.GetOrAdd(key, () => _userManager.Users.SingleOrDefault(u => u.Id == userId), Options);
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
        if (string.IsNullOrEmpty(roleName)) return null;
        roleName = roleName.ToUpperInvariant();
        var result = await _userManager.Users
             .Where(x => x.UserRoleTenants.Any(y => y.Role.NormalizedName == roleName))
             .Include(x => x.UserRoleTenants)
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

    public async Task<ApplicationUserDto> GetApplicationUserDto(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetApplicationUserDto:{userId}";
        var result = await _cache.GetOrAddAsync(key,
            async () => await _userManager.Users.Where(x => x.Id == userId).Include(x => x.UserRoleTenants)
                .ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation) ?? new ApplicationUserDto(), Options);
        return result;
    }

    public async Task<List<ApplicationUserDto>?> GetUsers(string? tenantId, CancellationToken cancellation = default)
    {
        //TODO add pagination, change logic as map with tenant id & name kind of as per customusermanagerrole
        var key = $"GetApplicationUserDtoListWithTenantId:{tenantId}";
        Func<string?, CancellationToken, Task<List<ApplicationUserDto>?>> getUsersByTenantId = async (tenantId, token) =>
        {
            if (string.IsNullOrEmpty(tenantId))
            {//TODO this should not be in real time ,it explodes the system
                return await _userManager.Users.Take(20).Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                       .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync();
            }
            else
            {
                return await _userManager.Users.Where(x => x.TenantId == tenantId).Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                      .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync();
            }
        };
        var result = await _cache.GetOrAddAsync(key, () => getUsersByTenantId(tenantId, cancellation), Options);
        return result;
    }


    public async Task<TenantDto> GetTenantsOfUser(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetTenantsOfUser:{userId}";
        var result = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.Where(x => x.Id == userId).Select(x => x.UserRoleTenants.Select(ur => ur.Tenant)).ProjectTo<TenantDto>(_mapper.ConfigurationProvider).FirstAsync(cancellation), Options);
        return result;
    }
}