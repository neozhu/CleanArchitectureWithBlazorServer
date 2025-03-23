﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
    }

    public string? DisplayName { get; set; }
    public string? Provider { get; set; } = "Local";
    public string? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }

    public string? ProfilePictureDataUrl { get; set; }

    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }

    public string? SuperiorId { get; set; } = null;
    public ApplicationUser? Superior { get; set; } = null;
    public DateTime? Created { get; set; }
    public string? CreatedBy { get; set; }
    public ApplicationUser? CreatedByUser { get; set; } = null;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public ApplicationUser? LastModifiedByUser { get; set; } = null;

    public string? TimeZoneId { get; set; }
    public string? LanguageCode { get; set; }
}