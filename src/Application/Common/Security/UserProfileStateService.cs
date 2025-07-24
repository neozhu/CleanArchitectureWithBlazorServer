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
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserProfileStateService> _logger;

    public UserProfileStateService(
        IMapper mapper,
        IServiceScopeFactory scopeFactory,
        IFusionCache fusionCache,
        ILogger<UserProfileStateService> logger)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _fusionCache = fusionCache;
        _logger = logger;
    }

    /// <summary>
    /// Loads and initializes the user profile from the database.
    /// </summary>
    public async Task InitializeAsync(string userName)
    {
        try
        {
            var key = GetApplicationUserCacheKey(userName);
            var result = await _fusionCache.GetOrSetAsync(
                key,
                async _ =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    
                    return await userManager.Users
                                .Where(x => x.UserName == userName)
                                .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                                .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync();
                },
                RefreshInterval);

            if (result is not null)
            {
                _userProfile = result.ToUserProfile();
                NotifyStateChanged();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize user profile for {UserName}", userName);
            throw;
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
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        
        try
        {
            RemoveApplicationUserCache(userName);
            await InitializeAsync(userName);
            _logger.LogDebug("User profile refreshed for {UserName}", userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh user profile for {UserName}", userName);
            throw;
        }
    }

    /// <summary>
    /// Updates the user profile and clears the cache.
    /// </summary>
    public void UpdateUserProfile(string userName, string? profilePictureDataUrl, string? fullName, string? phoneNumber, string? timeZoneId, string? languageCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        
        _userProfile.ProfilePictureDataUrl = profilePictureDataUrl;
        _userProfile.DisplayName = fullName;
        _userProfile.PhoneNumber = phoneNumber;
        _userProfile.TimeZoneId = timeZoneId;
        _userProfile.LanguageCode = languageCode;
        
        RemoveApplicationUserCache(userName);
        NotifyStateChanged();
    }

    public event Func<Task>? OnChange;

    private async void NotifyStateChanged()
    {
        try
        {
            if (OnChange != null)
            {
                await OnChange.Invoke();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while notifying state change");
        }
    }

    private string GetApplicationUserCacheKey(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        return $"GetApplicationUserDto:{userName}";
    }

    public void RemoveApplicationUserCache(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        _fusionCache.Remove(GetApplicationUserCacheKey(userName));   
    }

    public void Dispose()
    {
        // No longer holding long-lived scope, nothing to dispose
        GC.SuppressFinalize(this);
    }
}

