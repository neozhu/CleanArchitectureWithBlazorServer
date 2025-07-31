using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IUserProfileState following Blazor state management best practices.
/// Uses immutable UserProfile snapshots with precise event notifications.
/// </summary>
public class UserProfileState : IUserProfileState, IDisposable
{
    // Cache refresh interval of 60 seconds
    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);

    // Current user profile state (immutable snapshot)
    private UserProfile _currentValue = UserProfile.Empty;
    private string? _currentUserId;

    // Concurrency control
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    // Dependencies
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserProfileState> _logger;

    public UserProfileState(
        IMapper mapper,
        IServiceScopeFactory scopeFactory,
        IFusionCache fusionCache,
        ILogger<UserProfileState> logger)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _fusionCache = fusionCache;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user profile snapshot (immutable).
    /// </summary>
    public UserProfile Value => _currentValue;

    /// <summary>
    /// Event triggered when the user profile changes.
    /// Subscribers receive the new UserProfile snapshot.
    /// </summary>
    public event EventHandler<UserProfile>? Changed;

    /// <summary>
    /// Ensures the user profile is initialized for the given user ID.
    /// Only loads from database on first call or when user changes.
    /// </summary>
    public async Task EnsureInitializedAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return;

        // Check if already initialized for this user
        if (_currentUserId == userId && _currentValue != UserProfile.Empty)
            return;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring lock
            if (_currentUserId == userId && _currentValue != UserProfile.Empty)
                return;

            var result = await LoadUserProfileFromDatabaseAsync(userId, cancellationToken);

            if (result is not null)
            {
                var newProfile = result.ToUserProfile();
                _currentUserId = userId;
                SetInternal(newProfile);
            }
            else
            {
                _currentUserId = userId;
                SetInternal(UserProfile.Empty with { UserId = userId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize user profile for {UserId}", userId);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Refreshes the user profile by clearing cache and reloading from database.
    /// </summary>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_currentUserId))
            return;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            ClearCacheInternal(_currentUserId);
            
            var result = await LoadUserProfileFromDatabaseAsync(_currentUserId, cancellationToken);

            if (result is not null)
            {
                var newProfile = result.ToUserProfile();
                SetInternal(newProfile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh user profile for {UserId}", _currentUserId);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Sets a new user profile directly (for local updates after database changes).
    /// </summary>
    public void Set(UserProfile userProfile)
    {
        ArgumentNullException.ThrowIfNull(userProfile);
        _currentUserId = userProfile.UserId;
        SetInternal(userProfile);
        ClearCacheInternal(userProfile.UserId);
    }

    /// <summary>
    /// Updates specific fields locally without database access.
    /// </summary>
    public void UpdateLocal(
        string? profilePictureDataUrl = null,
        string? displayName = null,
        string? phoneNumber = null,
        string? timeZoneId = null,
        string? languageCode = null)
    {
        if (_currentValue == UserProfile.Empty)
        {
            return;
        }

        var updatedProfile = _currentValue with
        {
            ProfilePictureDataUrl = profilePictureDataUrl ?? _currentValue.ProfilePictureDataUrl,
            DisplayName = displayName ?? _currentValue.DisplayName,
            PhoneNumber = phoneNumber ?? _currentValue.PhoneNumber,
            TimeZoneId = timeZoneId ?? _currentValue.TimeZoneId,
            LanguageCode = languageCode ?? _currentValue.LanguageCode
        };

        SetInternal(updatedProfile);
        ClearCacheInternal(_currentValue.UserId);
    }

    /// <summary>
    /// Clears the cache for the current user.
    /// </summary>
    public void ClearCache()
    {
        if (!string.IsNullOrWhiteSpace(_currentUserId))
        {
            ClearCacheInternal(_currentUserId);
        }
    }

    private void SetInternal(UserProfile newProfile)
    {
        var oldProfile = _currentValue;
        _currentValue = newProfile;

        // Trigger event if profile actually changed
        if (!ReferenceEquals(oldProfile, newProfile))
        {
            Changed?.Invoke(this, newProfile);
        }
    }

    private string GetApplicationUserCacheKey(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return $"GetApplicationUserDto:UserId:{userId}";
    }

    private void ClearCacheInternal(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return;
        
        _fusionCache.Remove(GetApplicationUserCacheKey(userId));
    }

    /// <summary>
    /// Loads user profile data from database with caching.
    /// </summary>
    private async Task<ApplicationUserDto?> LoadUserProfileFromDatabaseAsync(string userId, CancellationToken cancellationToken = default)
    {
        var key = GetApplicationUserCacheKey(userId);
        return await _fusionCache.GetOrSetAsync(
            key,
            async _ =>
            {
                using var scope = _scopeFactory.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                return await userManager.Users
                            .Where(x => x.Id == userId)
                            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                            .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(cancellationToken);
            },
            RefreshInterval);
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
        GC.SuppressFinalize(this);
    }
}
