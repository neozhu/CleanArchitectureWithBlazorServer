using System.Linq;
using CleanArchitecture.Blazor.Application.Constants.Role;
using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Enums;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public class CustomUserManager : UserManager<ApplicationUser>
{
    public const string defaultTenantId = "";//todo make it loaded as per db
    private readonly CustomRoleManager _roleManager;
    private readonly IServiceProvider _serviceProvider;
    public CustomUserManager(
         IUserStore<ApplicationUser> store,
         IOptions<IdentityOptions> optionsAccessor,
         IPasswordHasher<ApplicationUser> passwordHasher,
         IEnumerable<IUserValidator<ApplicationUser>> userValidators,
         IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
         ILookupNormalizer keyNormalizer,
         IdentityErrorDescriber errors,
         IServiceProvider services,
         ILogger<UserManager<ApplicationUser>> logger, CustomRoleManager roleManager)
         : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _roleManager = roleManager;
        _serviceProvider = services;
    }
    /*
    public async override Task<IdentityResult> CreateAsync(ApplicationUser user)
    {
        return await CreateAsync(user, null);
    }
    public async override Task<IdentityResult> CreateAsync(ApplicationUser user, string password = null)
    {
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = defaultTenantId;
        if (user.UserRoles == null || !user.UserRoles.Any())
            return await CreateWithDefaultRolesAsync(user, tenantId: user.TenantId);
        var result = string.IsNullOrEmpty(password) ? await base.CreateAsync(user) : await base.CreateAsync(user, password);
        return result;
    }
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string> roles, string tenantId = null, string password = null)
    {
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = defaultTenantId;
        return await CreateAsync(user, roles, user.TenantId);
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string> roles, string tenantId)
    {
        if (string.IsNullOrEmpty(tenantId)) tenantId = defaultTenantId;
        if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, tenantId);
        user.UserRoles = new List<ApplicationUserRole>();
        roles.ForEach(c =>
        {
            var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
            if (roleId != null) user.UserRoles.Add(new ApplicationUserRole() { RoleId = roleId, TenantId = tenantId });
        });

        return await CreateAsync(user, null, roles, tenantId);
    }


    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password, List<string> roles)
    {
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = defaultTenantId;
        if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, user.TenantId);
        user.UserRoles = new List<ApplicationUserRole>();
        roles.ForEach(c =>
        {
            var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
            if (roleId != null) user.UserRoles.Add(new ApplicationUserRole() { RoleId = roleId, TenantId = tenantId });
        });

        return await CreateAsync(user, password, roles, tenantId);
    }
    */
    public async Task<IdentityResult> CreateWithDefaultRolesAsync(ApplicationUser user, string tenantId = null, string password = null)
    {
        return await CreateAsync(user, new List<string> { RoleName.DefaultRole1 }, tenantId, password);
    }


    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string> roles = null, string tenantId = null, string password = null)
    {
        if (string.IsNullOrEmpty(tenantId)) tenantId = defaultTenantId;
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = tenantId;//this overrides already assigned tenant,had to make sure
        if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, user.TenantId, password);
        user.UserRoles = new List<ApplicationUserRole>();//here it ignores already exisitng UserRoles //TODO need to tink of this
        roles.ForEach(c =>
        {
            var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
            if (roleId != null) user.UserRoles.Add(new ApplicationUserRole() { RoleId = roleId, TenantId = user.TenantId });
        });

        var result = string.IsNullOrEmpty(password) ? await base.CreateAsync(user) : await base.CreateAsync(user, password);
        return result;
    }

    public async Task<int?> RolesUpdateInsert(ApplicationUser user, IEnumerable<string> roleNames)
    {
        if (user == null || string.IsNullOrEmpty(user.TenantId) || !Guid.TryParse(user.TenantId, out Guid id1)
            || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return null;
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Replace with your DbContext
        roleNames = roleNames.Select(x => x.Trim().TrimStart().TrimEnd().ToUpper())
            .Where(str => !string.IsNullOrEmpty(str)).Distinct()
            .GroupBy(i => i).Select(x => x.Key).ToList();
        if (roleNames.Any())
        {
            var existingAll = dbContext.UserRoles.Where(role => role.TenantId == user.TenantId);
            //todo might need to include Role also to get name
            var existing = existingAll.Where(x => roleNames.Contains(x.Role.NormalizedName!));
            var changesTriggered = false;
            if (existing.All(x => x.IsActive != user.IsUserTenantRolesActive))//existing update
            {
                existing.ForEach(x =>
                {
                    x.IsActive = user.IsUserTenantRolesActive;
                    dbContext.UserRoles.Attach(x);
                    dbContext.Entry(x).State = EntityState.Modified;
                });
                changesTriggered = true;
            }

            var toInsert = roleNames.Except(existingAll.Select(x => x.Role.Name));
            var toRemove = existingAll.Select(x => x.Role.Name).Except(roleNames);

            if (toInsert.Any())
            {
                var toAdd = new List<ApplicationUserRole>();
                toInsert.ForEach(x =>
                {
                    var roleId = (dbContext.Roles.FirstOrDefault(r => r.NormalizedName == x.ToUpper()))?.Id;
                    if (string.IsNullOrEmpty(roleId)) return;
                    toAdd.Add(new ApplicationUserRole() { UserId = user.Id, TenantId = user.TenantId, RoleId = roleId });
                });
                await dbContext.UserRoles.AddRangeAsync(toAdd);
                changesTriggered = true;
            }
            if (toRemove.Any())
            {
                dbContext.UserRoles.RemoveRange(existingAll.Where(x => toRemove.Contains(x.Role.Name)));
                changesTriggered = true;
            }
            return changesTriggered ? await dbContext.SaveChangesAsync() : 0;
        }
        return 0;
    }
    public override async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string roleName)
    {
        if (user == null || string.IsNullOrEmpty(user.TenantId) || !Guid.TryParse(user.TenantId, out Guid id1)
            || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return null;
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Replace with your DbContext

        var existing = dbContext.UserRoles.Where(role => role.TenantId == user.TenantId && role.Role.Name == roleName);
        dbContext.UserRoles.RemoveRange(existing);
        return await dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : null;
    }

    public override async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        if (user == null || string.IsNullOrEmpty(user.TenantId) || !Guid.TryParse(user.TenantId, out Guid id1)
             || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return null;
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Replace with your DbContext

        var roleId = (await dbContext.Roles.FirstOrDefaultAsync(x => x.NormalizedName == roleName.ToUpper()))?.Id;
        if (string.IsNullOrEmpty(roleId)) return null;
        var newInserted = dbContext.UserRoles
            .AddAsync(new ApplicationUserRole() { UserId = user.Id, TenantId = user.TenantId, RoleId = roleId });
        return await dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : null;
    }

    public async Task<IList<ApplicationUserRole>> GetUserRoleTenantIdsAsync(string userId, string roleId = null, string tenantId = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Replace with your DbContext

        var query = dbContext.UserRoles.AsQueryable();
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
    public CustomRoleManager(
        IRoleStore<ApplicationRole> store,
        IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<CustomRoleManager> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }
    public async Task<ApplicationRole> FindByNameAsync(string roleName, TenantType type)
    {
        return await FindByNameAsync(roleName, (byte)type);
    }
    public async Task<ApplicationRole> FindByNameAsync(string roleName, byte tenantType)
    {
        return await Roles?.FirstOrDefaultAsync(r => r.Name == roleName && r.TenantType == tenantType);
    }
    public async Task<ApplicationRole> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        throw new NotImplementedException("Please use with tenantid");
    }

    //public async Task<IdentityRole> FindByNameAsync(string roleName)
    //{
    //    return await Roles?.FirstOrDefaultAsync(r => r.Name == roleName);
    //}
}
