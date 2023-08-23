using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace CleanArchitecture.Blazor.Domain.Identity;

public static class UserManagerExtensions
{
    public static async Task<int> RemoveFromRolesAsyncWithTenantName(
       string userId,
       string tenantName, IApplicationDbContext context,
       params string[] roles)
    {
        var tenant = await context.Tenants.FirstAsync(x => x.Name == tenantName);
        return tenant != null ? await RemoveFromRolesAsyncWithTenantId(userId, tenant.Id, context, roles) : 0;
    }
    public static async Task<int> RemoveFromRolesAsyncWithTenantId(
        string userId,
        string tenantId, IApplicationDbContext context,
        params string[] roles)
    {
        var itemsToRemove = context.UserRoles.Where(item => item.UserId == userId && item.TenantId == tenantId &&
        context.Roles.Where(r => roles.Contains(r.Name)).Contains(item.Role));
        context.UserRoles.RemoveRange(itemsToRemove);
        return await context.SaveChangesAsync();
    }

    public static async Task<int> UpdateRolesAsyncWithTenantId(
    string userId, string tenantId, IApplicationDbContext context
    , bool isActiveTenantAllRoles = false, params string[] newSelectedRoles)
    {
        //todo we can check for tenantType if it roles exists or not kind of more check
        var existingRoles = await context.UserRoles.Where(item => item.UserId == userId && item.TenantId == tenantId).Select(x => new { UserRole = x, x.RoleId, x.Role.Name }).ToListAsync();
        if (newSelectedRoles == null || !newSelectedRoles.Any())
        {
            if (!existingRoles.Any()) return 0;
            context.UserRoles.RemoveRange(existingRoles.Select(x => x.UserRole));
            var result = await context.SaveChangesAsync();
            Console.WriteLine($"Roles removed :{result}");
            return result;
        }
        var toAdd = newSelectedRoles.Except(existingRoles.Select(x => x.Name));
        var toRemove = existingRoles.Where(e => !newSelectedRoles.ToList().Contains(e.Name!)).ToList();

        if (toAdd.Any())
        {
            var toAddList = new List<ApplicationUserRole>();
            var toAddRoles = await context.Roles.Where(item => toAdd.Contains(item.Name)).Select(x => x.Id).ToListAsync();
            toAddRoles.ForEach(t => toAddList.Add(new ApplicationUserRole() { UserId = userId, TenantId = tenantId, RoleId = t, IsActive = isActiveTenantAllRoles }));
            await context.UserRoles.AddRangeAsync(toAddList);
            Console.WriteLine($"Adding {toAdd.Count()} rules({string.Join(",", toAdd)})");
        }

        if (toRemove.Any())
        {
            context.UserRoles.RemoveRange(toRemove.Select(x => x.UserRole));
            Console.WriteLine($"Removing {toRemove.Count():0} rules({string.Join(",", toRemove)})");
        }

        if (toAdd.Any() || toRemove.Any()|| (existingRoles != null && existingRoles.Any(e => e.UserRole.IsActive != isActiveTenantAllRoles)))
        {
            if (existingRoles != null && existingRoles.Any(e => e.UserRole.IsActive != isActiveTenantAllRoles))
            {
                var forActivationChange = await context.UserRoles.Where(item => item.UserId == userId && item.TenantId == tenantId).ToListAsync();
                forActivationChange?.ForEach(r => r.IsActive = isActiveTenantAllRoles);
            }
            var result = await context.SaveChangesAsync();
            Console.WriteLine($"Role changes count:{result},Add:{toAdd?.Count():0},Remove:{toRemove?.Count():0}");
            return result;
        }
        Console.WriteLine($"No Role changes");
        return 0;
    }


    public static async Task<int> AddToRolesAsyncWithTenantId(
        string userId,
        string tenantId, IApplicationDbContext context,
        params string[] roles)
    {
        var rolesList = await context.Roles.Where(x => roles.Contains(x.Name)).ToListAsync();
        if (rolesList != null && rolesList.Any())
        {
            var rolesToInsert = new List<ApplicationUserRole>();
            rolesList.ForEach(r => rolesToInsert.Add(new ApplicationUserRole() { UserId = userId, TenantId = tenantId, Role = r }));
            await context.UserRoles.AddRangeAsync(rolesToInsert);
            var resultCount = await context.SaveChangesAsync();
            return resultCount;
        }
        return 0;
    }
    public static async Task<int> AddToRolesAsyncWithTenantName(
        string userId,
        string tenantName, IApplicationDbContext context,
        params string[] roles)
    {

        var tenant = await context.Tenants.FirstAsync(x => x.Name == tenantName);
        return tenant != null ? await AddToRolesAsyncWithTenantId(userId, tenant.Id, context, roles) : 0;
    }

    /*
   #region PleaseUseWithTenantMethods
   static string _message = "Please use AddToRolesAsyncWithTenantId/Name or RemoveFromRolesAsyncWithTenantId/Name instead of this";
   public static Exception PleaseUseWithTenantMethods()
   {
       Console.WriteLine(_message);
       return new Exception(_message);
   }
   public static int AddToRoles(this CustomUserManager usermanager,
      ApplicationUser user, params string[] roles) => throw PleaseUseWithTenantMethods();
   public static async Task<int> AddToRolesAsync(this CustomUserManager usermanager,
      ApplicationUser user, params string[] roles) => throw PleaseUseWithTenantMethods();

   public static int RemoveFromRoles(this CustomUserManager usermanager,
      ApplicationUser user, params string[] roles) => throw PleaseUseWithTenantMethods();
   public static async Task<int> RemoveFromRolesAsync(this CustomUserManager usermanager,
      ApplicationUser user, params string[] roles) => throw PleaseUseWithTenantMethods();
   #endregion */
}
