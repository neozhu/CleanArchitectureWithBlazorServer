using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Identity;
public static class UserManagerExtensions
{
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
        if (tenant != null)
        {
            return await AddToRolesAsyncWithTenantId(userId, tenant.Id, context, roles);
        }
        return 0;
    }
}
