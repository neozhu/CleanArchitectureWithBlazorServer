using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

/// <summary>
/// Custom user manager implementation for multi-tenant application.
/// </summary>
public class MultiTenantUserManager : UserManager<ApplicationUser>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly ILogger<MultiTenantUserManager> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiTenantUserManager"/> class.
    /// </summary>
    /// <param name="store">The user store.</param>
    /// <param name="optionsAccessor">The identity options accessor.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="userValidators">The user validators.</param>
    /// <param name="passwordValidators">The password validators.</param>
    /// <param name="keyNormalizer">The key normalizer.</param>
    /// <param name="errors">The identity error describer.</param>
    /// <param name="services">The service provider.</param>
    /// <param name="roleManager">The role manager.</param>
    /// <param name="logger">The logger.</param>
    public MultiTenantUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UserManager<ApplicationUser>> logger,
        IHttpContextAccessor httpContextAccessor ,
        IApplicationDbContextFactory dbContextFactory ,
        ILogger<MultiTenantUserManager> auditLogger )
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _roleManager = roleManager;
        _httpContextAccessor = httpContextAccessor;
        _dbContextFactory = dbContextFactory;
        _logger = auditLogger;
    }

    /// <summary>
    /// Adds the specified user to the given roles.
    /// </summary>
    /// <param name="user">The user to add to roles.</param>
    /// <param name="roles">The roles to add the user to.</param>
    /// <returns>The <see cref="IdentityResult"/> of the operation.</returns>
    public override async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var tenantId = user.TenantId;
        var normalizedRoleNames = roles.Select(NormalizeName).ToList();

        var tenantRoles = await _roleManager.Roles
            .Where(r => normalizedRoleNames.Contains(r.NormalizedName) && r.TenantId == tenantId)
            .ToListAsync();

        if (tenantRoles.Count != roles.Count())
        {
            var missingRoles = roles.Except(tenantRoles.Select(r => r.Name), StringComparer.OrdinalIgnoreCase);
            return IdentityResult.Failed(new IdentityError
            {
                Code = "RoleNotFound",
                Description = $"Roles '{string.Join(", ", missingRoles)}' do not exist in the user's tenant."
            });
        }

        foreach (var role in tenantRoles.Where(x=>!string.IsNullOrEmpty(x.Name)))
        {
            var result = await AddToRoleAsync(user, role.Name ?? string.Empty);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// Adds the specified user to the given role.
    /// </summary>
    /// <param name="user">The user to add to the role.</param>
    /// <param name="roleName">The name of the role to add the user to.</param>
    /// <returns>The <see cref="IdentityResult"/> of the operation.</returns>
    public override async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        var tenantId = user.TenantId;
        var normalizedRoleName = NormalizeName(roleName);

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName && r.TenantId == tenantId);

        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "RoleNotFound",
                Description = $"Role '{roleName}' does not exist in the user's tenant."
            });
        }

        if (await IsInRoleAsync(user, role.Name!))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserAlreadyInRole",
                Description = $"User is already in role '{roleName}'."
            });
        }

        var userRoleStore = GetUserRoleStore();
        await userRoleStore.AddToRoleAsync(user, role.NormalizedName!, CancellationToken.None);

        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Checks if the specified user is in the given role.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns><c>true</c> if the user is in the role, otherwise <c>false</c>.</returns>
    public override async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrEmpty(roleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(roleName));

        var normalizedRoleName = NormalizeName(roleName);
        return await _roleManager.Roles.AnyAsync(r =>
            r.NormalizedName == normalizedRoleName &&
            r.TenantId == user.TenantId &&
            Context.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == r.Id));
    }
    

    /// <summary>
    /// Removes the specified user from the given role.
    /// </summary>
    /// <param name="user">The user to remove from the role.</param>
    /// <param name="role">The name of the role to remove the user from.</param>
    /// <returns>The <see cref="IdentityResult"/> of the operation.</returns>
    public override async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role)
    {
        var normalizedRoleName = NormalizeName(role);
        var userRoleStore = GetUserRoleStore();
        await userRoleStore.RemoveFromRoleAsync(user, normalizedRoleName, CancellationToken.None);

        return await UpdateUserAsync(user);
    }

    public override async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        var result = await base.CheckPasswordAsync(user, password);
        
        // Only log failed password checks to avoid duplicate login records
        if (!result)
        {
            var userName = user.UserName ?? "Unknown";
            var userId = user.Id ?? "Unknown";
            
            // Use the injected services to log the failed password check
            if (_httpContextAccessor != null && _dbContextFactory != null && _logger != null)
            {
                await LogPasswordCheckFailureAsync(userId, userName, _httpContextAccessor, _dbContextFactory, _logger);
            }
        }
        
        return result;
    }

    private async Task LogPasswordCheckFailureAsync(string userId, string userName, 
        IHttpContextAccessor httpContextAccessor, IApplicationDbContextFactory dbContextFactory, 
        ILogger<MultiTenantUserManager> logger)
    {
        await using var db = await dbContextFactory.CreateAsync();
        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                logger.LogWarning("HttpContext is null, cannot log password check failure for user {UserName}", userName);
                return;
            }

            // Extract client information
            var ipAddress = GetClientIpAddress(httpContext);
            var browserInfo = GetBrowserInfo(httpContext);

            // Create login audit for failed password check
            var loginAudit = new LoginAudit()
            {
                LoginTimeUtc = DateTime.UtcNow,
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                BrowserInfo = browserInfo,
                Provider = "PasswordCheck",
                Success = false
            };
            
            loginAudit.AddDomainEvent(new Domain.Events.LoginAuditCreatedEvent(loginAudit));
            
            // Save to database
            await db.LoginAudits.AddAsync(loginAudit);
            await db.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to log password check failure for user {UserName}", userName);
            // Don't throw - audit failure shouldn't break the password check process
        }
    }

    private string? GetClientIpAddress(HttpContext httpContext)
    {
        try
        {
            // Check for forwarded IP first (when behind proxy/load balancer)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // Take the first IP if there are multiple
                var firstIp = forwardedFor.Split(',').FirstOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(firstIp))
                    return firstIp;
            }

            // Check for real IP header
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
                return realIp;

            // Fall back to remote IP
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();

            // Handle localhost scenarios
            if (remoteIp == "::1" || remoteIp == "127.0.0.1")
            {
                return "127.0.0.1"; // Normalize localhost
            }

            return SanitizeInput(remoteIp);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to get client IP address");
            return null;
        }
    }

    private string SanitizeInput(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        // Remove newline characters and trim whitespace
        return input.Replace("\r", "").Replace("\n", "").Trim();
    }

    private string? GetBrowserInfo(HttpContext httpContext)
    {
        try
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
            if (string.IsNullOrEmpty(userAgent))
                return null;

            // Truncate if too long to prevent database issues
            return userAgent.Length > 1000 ? userAgent.Substring(0, 1000) : userAgent;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to get browser info");
            return null;
        }
    }

    private IUserRoleStore<ApplicationUser> GetUserRoleStore()
    {
        return Store as IUserRoleStore<ApplicationUser>
               ?? throw new NotSupportedException("The user store does not implement IUserRoleStore<ApplicationUser>.");
    }

    private ApplicationDbContext Context => (Store as UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, string, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>)?.Context ?? throw new InvalidOperationException("Context is not available.");
}
