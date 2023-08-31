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

}
