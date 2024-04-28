using System.Linq.Dynamic.Core;
using ActualLab.Fusion;
using ActualLab.Fusion.EntityFramework;
using ActualLab.Fusion.Extensions;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class OnlineUserTracker : IOnlineUserTracker
{
    private const string PREFIX = "U";
    private readonly IKeyValueStore _store;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Func<ApplicationUser, UserProfile> _toUserProfile;
    public OnlineUserTracker(IKeyValueStore store, IServiceScopeFactory scopeFactory)
    {
        _store = store;
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _toUserProfile = user => new UserProfile
        {
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            UserId = user.Id,
            IsActive = user.IsActive,
            PhoneNumber = user.PhoneNumber,
            Provider = user.Provider,
            ProfilePictureDataUrl = user.ProfilePictureDataUrl,
            SuperiorId = user.SuperiorId,
            SuperiorName = user.Superior?.UserName ?? string.Empty,
            TenantId = user.TenantId,
            TenantName = user.Tenant?.Name ?? string.Empty,
            AssignedRoles = user.UserRoles.Any() ? user.UserRoles.Select(x => x.Role.Name!).ToArray() : Array.Empty<string>(),
            DefaultRole = user.UserRoles.Any() ? user.UserRoles.First().Role.Name : string.Empty
        };
    }
    private DbShard _shard = DbShard.None;
    public async Task AddUser(string userId, CancellationToken cancellationToken = default)
    {
        if (Computed.IsInvalidating())
            _ = GetOnlineUsers();

       
        if (!string.IsNullOrEmpty(userId))
        {
            var key = $"{PREFIX}/{userId}";
            var val =await _store.TryGet<UserProfile>(_shard,key, cancellationToken);
            if (!val.HasValue)
            {
                var userDto = await _userManager.Users.Where(x => x.UserName == userId).Include(x => x.Tenant).Include(x => x.UserRoles).ThenInclude(x => x.Role)
               .Select(x => _toUserProfile(x)).FirstOrDefaultAsync(cancellationToken);
                await _store.Set(_shard, key, userDto, cancellationToken);
            }
           
        }
    }
    public async Task UpdateUser(string userId, CancellationToken cancellationToken = default)
    {
        if (Computed.IsInvalidating())
            _ = GetOnlineUsers();
        if (!string.IsNullOrEmpty(userId))
        {
            var key = $"{PREFIX}/{userId}";
            var userDto = await _userManager.Users.Where(x => x.UserName == userId).Include(x => x.Tenant).Include(x => x.UserRoles).ThenInclude(x => x.Role)
                .Select(x => _toUserProfile(x)).FirstOrDefaultAsync();
            await _store.Set(_shard, key, userDto);
        }
    }
    public async Task<Dictionary<string, UserProfile>> GetOnlineUsers(CancellationToken cancellationToken = default)
    {
        if (Computed.IsInvalidating())
            return default!;
        var keys=await _store.ListKeySuffixes(_shard, PREFIX, PageRef.New<string>(int.MaxValue));
        var result = new Dictionary<string, UserProfile>();
        foreach (var key in keys)
        {
            var userProfile = await _store.Get<UserProfile>(_shard, $"{PREFIX}{key}", cancellationToken);
            if (userProfile != null)
            {
                result[key] = userProfile;
            }
        }

        return result;
    }

    public async Task RemoveUser(string userId, CancellationToken cancellationToken = default)
    {
        if (Computed.IsInvalidating())
            _ = GetOnlineUsers();

        await _store.Remove(_shard, $"{PREFIX}/{userId}");
    }
}
