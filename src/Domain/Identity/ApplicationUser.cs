// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? Provider { get; set; } = "Google";
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
    [Column(TypeName = "text")]
    public string? ProfilePictureDataUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
    public virtual ICollection<ApplicationUserRoleTenant> UserRoleTenants { get; set; } = new List<ApplicationUserRoleTenant> ();
    [Description("Is User-Tenant Roles Active")][NotMapped] public bool IsUserTenantRolesActive { get; set; } = true;
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }

    public string? SuperiorId { get; set; } = null;
    [ForeignKey("SuperiorId")]
    public ApplicationUser? Superior { get; set; } = null;
    public ApplicationUser() : base()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoleTenants = new HashSet<ApplicationUserRoleTenant>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
    }
    //public ApplicationUser(ApplicationUser source)
    //{
    //    if (source == null)
    //       throw new ArgumentNullException(nameof(source));
    //    DisplayName
    //}

    //below is not working, need to check...taking long time and then become stuck
    //public ApplicationUser(ApplicationUser source)//
    //{
    //    if (source == null)
    //        throw new ArgumentNullException(nameof(source));

    //    Type type = typeof(ApplicationUser);
    //    PropertyInfo[] properties = type.GetProperties();

    //    foreach (PropertyInfo property in properties)
    //    {
    //        property.SetValue(this, property.GetValue(source));
    //    }
    //}

}
