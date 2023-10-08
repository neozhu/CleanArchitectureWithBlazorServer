﻿using System.Linq;
using System.Linq.Dynamic.Core;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Constants.Permission;
using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Enums;
using Common;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static CleanArchitecture.Blazor.Application.Constants.Permission.Permissions;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public class CustomUserManager : UserManager<ApplicationUser>
{
    readonly List<string> defaultRoles = new() { RoleNamesEnum.Patient.ToString() };

    public const string DefaultTenantId = "";//todo make it loaded as per db
    private readonly CustomRoleManager _roleManager;
    //  private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext dbContext;
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
        // _serviceProvider = services;
        dbContext = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
    public async Task<IdentityResult> CreateWithDefaultRolesAsync(ApplicationUser user, string? tenantId = null, string? password = null)
    {
        return await CreateAsync(user, defaultRoles, tenantId, password);
    }
    public override async Task<ApplicationUser?> FindByIdAsync(string userId)
    {
        return await dbContext.Users
            .Include(x => x.UserRoleTenants)//.ThenInclude(x =>  x.Role).Include(x => x.UserRoleTenants).ThenInclude(x => x.Tenant)
            .Include(x => x.UserClaims).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
    //public async Task<ApplicationUser?> FindByIdAsyncNoTracking(string userId)
    //{
    //    return await Users.AsNoTracking()
    //        .Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
    //        .Include(x => x.UserRoleTenants).ThenInclude(x => x.Tenant)
    //        .Include(x => x.UserClaims)
    //        .FirstOrDefaultAsync(x => x.Id == userId);
    //}
    public async Task<List<ApplicationUserRoleTenant>> GetUserRoles(string userId)
    {
        return await dbContext.UserRoles.AsNoTracking()
            .Include(x => x.Role)
            .Include(x => x.Tenant)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
    public override async Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        if (userName.IsNullOrEmptyAndTrimSelf()) return null;
        userName = userName.ToUpperInvariant();
        // var user = await base.FindByNameAsync(userName);
        var result = await dbContext.Users.AsNoTracking()
            .Include(x => x.UserRoleTenants)//.ThenInclude(x => x.Role).Include(x => x.UserRoleTenants).ThenInclude(x => x.Tenant)
            .Include(x => x.UserClaims)
            //.Select(x => new ApplicationUser(x)
            //{
            //    UserClaims=x.UserClaims,
            //    UserRoleTenants = x.UserRoleTenants
            //    .Select(urt => new ApplicationUserRoleTenant()
            //    { UserId = urt.UserId, TenantId = urt.TenantId, RoleId = urt.RoleId, UserName = x.UserName, RoleName = urt.Role.Name, TenantName = urt.Tenant.Name }).ToList()
            //})
            .FirstOrDefaultAsync(x => x.NormalizedUserName == userName);
        if (result != null && result.UserRoleTenants != null && result.UserRoleTenants.Any())
            foreach (var item in result.UserRoleTenants)
            {
              //  item.UserName = item.User?.UserName;//this is also redundant info but still somewhere else might needed so kept it
                item.User = null;
                //item.RoleName = item.Role.Name;//these are null bcz not included
                //item.TenantName = item.Tenant.Name;
            };
        return result;
    }
    public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
    {
        // Add your custom logic here before calling the base method
        // For example, you can validate user data or perform additional tasks.
        try
        {
            //next madhu
            //dbContext.Users.Attach(user);
            //var result = dbContext.Users.Update(user);
            //await dbContext.SaveChangesAsync();
            //return Result;
            user.UserRoleTenants = null;//temporary fix to avoid The instance of entity type 'Tenant' cannot be tracked because another instance with the key value '{Id: 3b8ec9a3-04b3-4585-8796-99f44dd64ed9}' is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached
            var result2 = await base.UpdateAsync(user);
            return result2; ;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return IdentityResult.Failed(new IdentityError() { Description = e.ToString() });
        }
    }
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string>? roles = null, string? tenantId = null, string? password = null)
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

        var result = password.IsNullOrEmptyAndTrimSelf() ? await base.CreateAsync(user) : await base.CreateAsync(user, password!);
        return result;
    }

    public async Task<int?> RolesUpdateInsert(ApplicationUser user, IEnumerable<string> roleNames)
    {
        if (user == null || user.TenantId.IsNullOrEmptyAndTrimSelf() || !Guid.TryParse(user.TenantId, out Guid id1)
            || user.Id.IsNullOrEmptyAndTrimSelf() || !Guid.TryParse(user.Id, out Guid id)) return null;

        roleNames = roleNames.Select(x => x.Trim().TrimStart().TrimEnd().ToUpper())
            .Where(str => !str.IsNullOrEmpty()).Distinct()
            .GroupBy(i => i).Select(x => x.Key).ToList();
        if (roleNames.Any())
        {
            var existingAll = dbContext.UserRoles.Where(role => role.UserId == user.Id && role.TenantId == user.TenantId);
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

            var toInsert = roleNames.Except(existingAll.Select(x => x.Role.NormalizedName));
            var toRemove = existingAll.Select(x => x.Role.NormalizedName).ToList().Except(roleNames);

            if (toInsert.Any())
            {
                var toAdd = new List<ApplicationUserRoleTenant>();
                toInsert.ForEach(x =>
                {
                    var roleId = (dbContext.Roles.FirstOrDefault(r => r.NormalizedName == x!.ToUpper()))?.Id;
                    if (string.IsNullOrEmpty(roleId)) return;
                    toAdd.Add(new ApplicationUserRoleTenant() { UserId = user.Id, TenantId = user.TenantId, RoleId = roleId });
                });
                await dbContext.UserRoles.AddRangeAsync(toAdd);
                changesTriggered = true;
            }
            if (toRemove.Any())
            {
                dbContext.UserRoles.RemoveRange(existingAll.Where(x => toRemove.Contains(x.Role.NormalizedName)));
                changesTriggered = true;
            }
            return changesTriggered ? await dbContext.SaveChangesAsync() : 0;
        }
        return 0;
    }
    public override async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string roleName)
    {
        if (string.IsNullOrEmpty(roleName) || user == null || string.IsNullOrEmpty(user.TenantId) || !Guid.TryParse(user.TenantId, out Guid id1)
            || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return IdentityResult.Failed();
        roleName = roleName.ToUpperInvariant();
        var existing = dbContext.UserRoles.Where(role => role.TenantId == user.TenantId && role.Role.Name == roleName);
        dbContext.UserRoles.RemoveRange(existing);
        return await dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public override async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        if (string.IsNullOrEmpty(roleName) || user == null || string.IsNullOrEmpty(user.TenantId) || !Guid.TryParse(user.TenantId, out Guid id1)
             || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return IdentityResult.Failed();
        roleName = roleName.ToUpperInvariant();
        var roleId = (await dbContext.Roles.FirstOrDefaultAsync(x => x.NormalizedName == roleName.ToUpper()))?.Id;
        if (string.IsNullOrEmpty(roleId)) return IdentityResult.Failed();
        var newInserted = dbContext.UserRoles
            .AddAsync(new ApplicationUserRoleTenant() { UserId = user.Id, TenantId = user.TenantId, RoleId = roleId });
        return await dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IList<ApplicationUserRoleTenant>> GetUserRoleTenantIdsAsync(string userId, string? roleId = null, string? tenantId = null)
    {
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
