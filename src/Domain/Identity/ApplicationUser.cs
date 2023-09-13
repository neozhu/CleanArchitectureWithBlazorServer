// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? Provider { get; set; } = "Local";
    [NotMapped]
    public List<string>? TenantsActive { get { return this.UserRoles?.Where(x => x.IsActive).Select(x => x.TenantId)?.Distinct().ToList(); } }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
    [Column(TypeName = "text")]
    public string? ProfilePictureDataUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    [Description("Is User-Tenant Roles Active")][NotMapped] public bool IsUserTenantRolesActive { get; set; } = true;
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }

    public string? SuperiorId { get; set; } = null;
    [ForeignKey("SuperiorId")]
    public ApplicationUser? Superior { get; set; } = null;
    public ApplicationUser() : base()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
    }


}
