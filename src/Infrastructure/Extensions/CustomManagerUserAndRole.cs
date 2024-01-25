using System.Diagnostics.Tracing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Constants.Permission;
using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using Common;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using CleanArchitecture.Blazor.Infrastructure.Common.Extensions;
using static CleanArchitecture.Blazor.Infrastructure.Constants.Permission.Permissions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public class Repository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    // private readonly IServiceProvider _services;
    public Repository(IServiceProvider services)
    {
        //   _services = services;
        _dbContext = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public Repository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task<int> UpdateColumnAsync<TProperty>(T entity, Expression<Func<T, TProperty>> propertyExpression, TProperty value)
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
        var entry = _dbContext.Entry(entity);
        entry.Property(propertyName).CurrentValue = value;
        entry.Property(propertyName).IsModified = true;
        entry.CurrentValues.SetValues(entity);
        return await _dbContext.SaveChangesAsync();
    }
}
public class CustomUserManager : UserManager<ApplicationUser>
{
    readonly List<string> _defaultRoles = new() { RoleNamesEnum.PATIENT.ToString() };
    private Repository<ApplicationUser> _repository;
    public const string DefaultTenantId = "";//todo make it loaded as per db
    private readonly CustomRoleManager _roleManager;
    private readonly IServiceProvider _serviceProvider;
    private ApplicationDbContext _dbContext;
    private readonly IServiceScopeFactory _scopeFactory;
    public CustomUserManager(ApplicationDbContext context,
         IUserStore<ApplicationUser> store,
         IOptions<IdentityOptions> optionsAccessor,
         IPasswordHasher<ApplicationUser> passwordHasher,
         IEnumerable<IUserValidator<ApplicationUser>> userValidators,
         IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
         ILookupNormalizer keyNormalizer,
         IdentityErrorDescriber errors,
         IServiceProvider services,
         ILogger<UserManager<ApplicationUser>> logger, CustomRoleManager roleManager, IServiceScopeFactory scopeFactory)
         : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _scopeFactory = scopeFactory;
        _roleManager = roleManager;
        _serviceProvider = services;
        _dbContext = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _repository = new Repository<ApplicationUser>(_dbContext);
    }
    public async Task<IdentityResult> CreateWithDefaultRolesAsync(ApplicationUser user, string? tenantId = null, string? password = null)
    {
        return await CreateAsync(user, _defaultRoles, tenantId, password);
    }

    public int UpdateIsLive(string userId, bool isLive = false)
    {
        return UpdateColumn(userId, nameof(ApplicationUser.IsLive), isLive);
    }

    //TODO change it to async
    public int UpdateColumn(string userId, string columnName, object newValue)
    {
        using (_dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
            var entity = _dbContext.Users.Find(userId);

            if (entity != null)
            {
                // Use reflection to set the property value based on the column name
                var propertyInfo = entity.GetType().GetProperty(columnName);

                if (propertyInfo != null)
                {
                    var convertedValue = Convert.ChangeType(newValue, propertyInfo.PropertyType);
                    propertyInfo.SetValue(entity, convertedValue);
                    _dbContext.Entry(entity).State = EntityState.Modified;
                    return _dbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Invalid column name.");
                }
            }
            else
            {
                throw new ArgumentException("Entity not found.");
            }
        }
    }

    public override async Task<ApplicationUser?> FindByIdAsync(string userId)
    {
        return await FindByNameOrId(userId: Guid.Parse(userId.TrimSelf()));
    }
    public async Task<ApplicationUser?> FindByNameForLocalAccountAsync(string userName)
    {//Since local account validation expects hash and all so get it from default.In real time its performance overhead.
     //TODO need to validate which is best in performance

        return await FindByNameAsync(userName);
        // return await base.FindByNameAsync(userName);
    }
    public override async Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        return await FindByNameOrId(userName: userName);
    }
    public async Task<List<Tenant>> GetAllTenants()
    {
        return await _dbContext.Tenants.Where(x => x.Active).ToListAsync();
    }
    public async Task<ApplicationUser?> FindByNameOrId(string userName = "", Guid? userId = null)
    {
        if (userName.IsNullOrEmptyAndTrimSelf() && !userId.HasValue) return null;

        string searchCriteria = userName; // Replace with your search criteria
        bool searchById = false; // Set to true if searching by Id, false if searching by UserName


        if (userName.IsNullOrEmptyAndTrimSelf() && userId.HasValue) { searchById = true; searchCriteria = userId?.ToString(); }

        using (_dbContext)
        {
            var query = _dbContext.Users
                .Where(user => (searchById && user.Id == searchCriteria) || (!searchById && user.UserName == searchCriteria))
                .AsNoTracking()
                .Select(user => new ApplicationUser
                {
                    UserName = user.UserName,
                    UserClaims = _dbContext.UserClaims.Where(uc => uc.UserId == user.Id).ToList(),
                    //TODO change below as join operator
                    UserRoleTenants = _dbContext.UserRoles.Where(urt => urt.UserId == user.Id).Select(u => new ApplicationUserRoleTenant
                    {
                        TenantId = u.TenantId,
                        TenantName = u.Tenant.Name,
                        RoleId = u.RoleId,
                        RoleName = u.Role.Name
                    }).ToList(),
                    Id = user.Id,
                    SecurityStamp = user.SecurityStamp,
                    DisplayName = user.DisplayName,
                    Provider = user.Provider,
                    TenantId = user.TenantId,
                    TenantName = user.TenantName,
                    ProfilePictureDataUrl = user.ProfilePictureDataUrl,
                    IsActive = user.IsActive,
                    IsLive = user.IsLive,
                    RefreshToken = user.RefreshToken,
                    RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                    Logins = user.Logins,
                    Tokens = user.Tokens,
                    SuperiorId = user.SuperiorId,
                    Superior = user.Superior//this also can be replaced by superior name kind of
                });

            var applicationUser = await query.FirstOrDefaultAsync();
            return applicationUser;
        }
    }

    public async Task<ApplicationUser?> FindByNameOrIdFullUserObject(string userName = "", Guid? userId = null)
    {
        if (userName.IsNullOrEmptyAndTrimSelf() && !userId.HasValue) return null;

        string searchCriteria = userName; // Replace with your search criteria
        bool searchById = false; // Set to true if searching by Id, false if searching by UserName

        if (userName.IsNullOrEmptyAndTrimSelf() && userId.HasValue) { searchById = true; searchCriteria = userId?.ToString(); }

        using (_dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
            var applicationUser = await _dbContext.Users
                .Include(x => x.UserClaims)
                .Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                .Include(x => x.UserRoleTenants).ThenInclude(x => x.Tenant)
                .Where(user => (searchById && user.Id == searchCriteria) || (!searchById && user.UserName == searchCriteria))
                .FirstOrDefaultAsync();
            return applicationUser;
        }
    }
    public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
    {//here role update not happening,for that separate code after this completion

        // Add your custom logic here before calling the base method
        // For example, you can validate user data or perform additional tasks.
        var existingUserRoleTenants = user?.UserRoleTenants;
        try
        {

            //todo need to verify the perfection
            user.UserRoleTenants = null;//if this not done then tracking issue will block,so this is improtant to keep
            using (_dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var result = _dbContext.Users.Update(user);
                var rrr = await _dbContext.SaveChangesAsync();
                user.UserRoleTenants = existingUserRoleTenants;
                return IdentityResult.Success;
            }

        }
        catch (Exception e)
        {
            if (user != null && existingUserRoleTenants != null)
                user.UserRoleTenants = existingUserRoleTenants;
            Console.WriteLine(e.ToString());
            return IdentityResult.Failed(new IdentityError() { Description = e.ToString() });
        }
    }
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string>? roles = null, string? tenantId = null, string? password = null)
    {
        try
        {
            if (tenantId.IsNullOrEmptyAndTrimSelf()) tenantId = DefaultTenantId;
            if (user.TenantId.IsNullOrEmptyAndTrimSelf()) user.TenantId = tenantId;//this overrides already assigned tenant,had to make sure
            if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, user.TenantId, password);
            user.UserRoleTenants = new List<ApplicationUserRoleTenant>();//here it ignores already existing UserRoleTenants //TODO need to think of this
            roles.ForEach(c =>
            {
                var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
                if (roleId != null)
                {
                    user.UserRoleTenants.Add(new ApplicationUserRoleTenant() { RoleId = roleId, TenantId = user.TenantId! });
                    /* This is required if default scopes for user level need to assign
                    var scopes = Perms.PermissionsAll.Find(x => x.roleOrType.Equals(c, StringComparison.InvariantCultureIgnoreCase)).permissions;
                    if (scopes != null && scopes.Any())
                        foreach (var scope in scopes)
                        {
                            base.AddClaimAsync(user, new Claim("Permissions", scope));
                        }
                    */
                }
            });
            using (_dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var result = password.IsNullOrEmptyAndTrimSelf() ? await base.CreateAsync(user) : await base.CreateAsync(user, password!);
                return result;
            }
            //TODO verify this works or not
            //    using var scope = _scopeFactory.CreateScope();
            //_dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //var result = password.IsNullOrEmptyAndTrimSelf() ? await base.CreateAsync(user) : await base.CreateAsync(user, password!);
            //return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString()); throw;
        }
    }

    public async Task<int?> RolesUpdateInsert(ApplicationUser user, IEnumerable<string> roleNames)
    {
        if (user == null || user.TenantId.IsNullOrEmptyAndTrimSelf() || !Guid.TryParse(user.TenantId, out Guid tenantId)
            || user.Id.IsNullOrEmptyAndTrimSelf() || !Guid.TryParse(user.Id, out Guid id)) return null;

        roleNames = roleNames.Select(x => x.Trim().TrimStart().TrimEnd().ToUpper())
            .Where(str => !str.IsNullOrEmpty()).Distinct()
            .GroupBy(i => i).Select(x => x.Key).ToList();
        using (_dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
            if (roleNames.Any())
            {
                var existingAllInCurrentTenant = _dbContext.UserRoles.Include(x => x.Role).Where(role => role.UserId == user.Id && role.TenantId == user.TenantId);
                var t1 = existingAllInCurrentTenant.ToList();
                //save happens ata time for one tenant,so other tenant will not fetch details
                var existing = existingAllInCurrentTenant.Where(x => roleNames.Contains(x.Role.NormalizedName!)).ToList();
                var changesTriggered = false;
                if (existing.All(x => x.IsActive != user.IsUserTenantRolesActive))//existing update
                {
                    existing.ForEach(x =>
                    {
                        x.IsActive = user.IsUserTenantRolesActive;
                        _dbContext.UserRoles.Attach(x);
                        _dbContext.Entry(x).State = EntityState.Modified;
                    });
                    changesTriggered = true;
                }

                var toInsert = roleNames.Except(existingAllInCurrentTenant.Select(x => x.Role.NormalizedName)).ToList();
                var toRemove = existingAllInCurrentTenant.Select(x => x.Role.NormalizedName).ToList().Except(roleNames).ToList();

                if (toInsert.Any())
                {
                    var toAdd = new List<ApplicationUserRoleTenant>();
                    toInsert.ForEach(x =>
                    {
                        var roleId = (_dbContext.Roles.FirstOrDefault(r => r.NormalizedName == x!.ToUpper()))?.Id;
                        if (string.IsNullOrEmpty(roleId)) return;
                        toAdd.Add(new ApplicationUserRoleTenant() { UserId = user.Id, TenantId = user.TenantId, RoleId = roleId });
                    });
                    await _dbContext.UserRoles.AddRangeAsync(toAdd);
                    changesTriggered = true;
                }
                if (toRemove.Any())
                {
                    _dbContext.UserRoles.RemoveRange(existingAllInCurrentTenant.Where(x => toRemove.Contains(x.Role.NormalizedName)));
                    changesTriggered = true;
                }
                return changesTriggered ? await _dbContext.SaveChangesAsync() : 0;
            }
        }
        return 0;
    }
    public override async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string roleName)
    {
        if (string.IsNullOrEmpty(roleName) || user == null || string.IsNullOrEmpty(user.TenantId) || !Guid.TryParse(user.TenantId, out Guid id1)
            || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return IdentityResult.Failed();
        roleName = roleName.ToUpperInvariant();
        using (_dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
            var existing = _dbContext.UserRoles.Where(role => role.TenantId == user.TenantId && role.Role.Name == roleName);
            _dbContext.UserRoles.RemoveRange(existing);
            return await _dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }
    }

    public override async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        if (string.IsNullOrEmpty(roleName) || user == null || string.IsNullOrEmpty(user.TenantId) || !Guid.TryParse(user.TenantId, out Guid id1)
             || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return IdentityResult.Failed();
        roleName = roleName.ToUpperInvariant();
        var roleId = (await _dbContext.Roles.FirstOrDefaultAsync(x => x.NormalizedName == roleName.ToUpper()))?.Id;
        if (string.IsNullOrEmpty(roleId)) return IdentityResult.Failed();
        var newInserted = _dbContext.UserRoles
            .AddAsync(new ApplicationUserRoleTenant() { UserId = user.Id, TenantId = user.TenantId, RoleId = roleId });
        return await _dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IList<ApplicationUserRoleTenant>> GetUserRoleTenantIdsAsync(string userId, string? roleId = null, string? tenantId = null)
    {
        var query = _dbContext.UserRoles.AsQueryable();
        if (!string.IsNullOrEmpty(tenantId))
            query = query.Where(role => role.TenantId == tenantId);
        if (!string.IsNullOrEmpty(roleId))
            query = query.Where(role => role.RoleId == roleId);
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(role => role.UserId == userId);
        return await query.ToListAsync();
    }

}

public class CustomRoleManager : RoleManager<ApplicationRole>
{
    // private readonly ApplicationDbContext _dbContext;
    public CustomRoleManager(
        IRoleStore<ApplicationRole> store,
        IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<CustomRoleManager> logger)//, IServiceProvider serviceProvider)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
        // _dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public async Task<ApplicationRole?> FindByNameAsync(string roleName, TenantTypeEnum type)
    {
        return await FindByNameAsync(roleName, (byte)type);
    }
    public async Task<ApplicationRole?> FindByNameAsync(string roleName, byte tenantType)
    {
        return roleName.IsNullOrEmptyAndTrimSelf()
            ? null
            : await Roles?.FirstOrDefaultAsync(r => r != null && r.Name! == roleName! && r.TenantType == tenantType);
    }
    //public async Task<ApplicationRole> AddToRoleAsync(ApplicationUser user, string roleName)
    //{
    //    throw new NotImplementedException("Please use with tenantId");
    //}


    //public async Task<IdentityRole> FindByNameAsync(string roleName)
    //{
    //    return await Roles?.FirstOrDefaultAsync(r => r.Name == roleName);
    //}
}














/*
 public class YourDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // Add other columns as needed
}

public class Repository<T> where T : class
{
    private YourDbContext _dbContext;

    public Repository(YourDbContext context)
    {
        _dbContext = context;
    }

    public void UpdateColumnAsync<TProperty>(T entity, Expression<Func<T, TProperty>> propertyExpression, TProperty value)
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
        var entry = _dbContext.Entry(entity);
        entry.Property(propertyName).IsModified = true;
        entry.CurrentValues.SetValues(entity);

        _dbContext.SaveChanges();
    }
}

class Program
{
    static void Main()
    {
        using (var _dbContext = new YourDbContext())
        {
            var repository = new Repository<User>(_dbContext);

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
            if (user != null)
            {
                repository.UpdateColumnAsync(user, u => u.FirstName, "NewFirstName");
            }
        }
    }
}
 */