// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Collections.Generic;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationRole : IdentityRole
{
    //public int PermissionLevel { get; set; }
    //public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public byte TenantType { get; set; } = (byte)Enums.TenantType.Patient;
    public byte Level { get; set; } = 1;
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    //add isActive,activatedBy,deactivatedBy
    public ApplicationRole() : base()
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
    }

    public ApplicationRole(string roleName, TenantType tenantType = Enums.TenantType.Patient) : base(roleName)
    {
        TenantType = (byte)tenantType;
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Description = roleName;
    }
    public ApplicationRole(string roleName, byte tenantType) : base(roleName)
    {
        TenantType = tenantType;
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Description = roleName;
    }

    public static List<ApplicationRole> CreateRolesForTenantType(List<string> roleNames, TenantType tenantType)
    {
        //todo need to add permissions
        List<ApplicationRole> result = new();
        foreach (var item in roleNames)
        {
            result.Add(new ApplicationRole(item.ToUpperInvariant(), tenantType));
        }
        return result;
    }
}
