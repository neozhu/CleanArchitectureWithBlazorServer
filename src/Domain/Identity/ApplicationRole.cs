// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationRole : IdentityRole, IAuditableEntity
{
    public ApplicationRole()
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
    }
    public string? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedById { get; set; }

}
