using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class UserProfileStateService : IDisposable
{
    // Cache refresh interval of 60 seconds
    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);

    // Internal user profile state
    private UserProfile _userProfile = new UserProfile { Email = "", UserId = "", UserName = "" };

    // Dependencies
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly IServiceScope _scope;

    public UserProfileStateService(
        IMapper mapper,
        IServiceScopeFactory scopeFactory,
        IFusionCache fusionCache)
    {
        _scope = scopeFactory.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _mapper = mapper;
        _fusionCache = fusionCache;
    }

    /// <summary>
    /// Loads and initializes the user profile from the database.
    /// </summary>
    public async Task InitializeAsync(string userName)
    {
        var key = GetApplicationUserCacheKey(userName);
        var result = await _fusionCache.GetOrSetAsync(
            key,
            _ => _userManager.Users
                        .Where(x => x.UserName == userName)
                        .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                        .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(),
            RefreshInterval);

        if (result is not null)
        {
            _userProfile = result.ToUserProfile();
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Gets or sets the current user profile.
    /// </summary>
    public UserProfile UserProfile
    {
        get => _userProfile;
        set
        {
            _userProfile = value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Refreshes the user profile by removing the cached value and reloading data from the database.
    /// </summary>
    public async Task RefreshAsync(string userName)
    {
        RemoveApplicationUserCache(userName);
        await InitializeAsync(userName);
    }

    /// <summary>
    /// Updates the user profile and clears the cache.
    /// </summary>
    public void UpdateUserProfile(string userName, string? profilePictureDataUrl, string? fullName, string? phoneNumber, string? timeZoneId, string? languageCode)
    {
        _userProfile.ProfilePictureDataUrl = profilePictureDataUrl;
        _userProfile.DisplayName = fullName;
        _userProfile.PhoneNumber = phoneNumber;
        _userProfile.TimeZoneId = timeZoneId;
        _userProfile.LanguageCode = languageCode;
        RemoveApplicationUserCache(userName);
        NotifyStateChanged();
    }

    public event Func<Task>? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();

    private string GetApplicationUserCacheKey(string userName)
    {
        return $"GetApplicationUserDto:{userName}";
    }

    public void RemoveApplicationUserCache(string userName)
    {
        _fusionCache.Remove(GetApplicationUserCacheKey(userName));
    }

    public void Dispose() => _scope.Dispose();
}

