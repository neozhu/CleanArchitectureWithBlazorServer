// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Common.Constants;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UserProfile GetUserProfileFromClaim(this
        ClaimsPrincipal claimsPrincipal
    )
    {
        if (!(claimsPrincipal.Identity?.IsAuthenticated ?? false))
        {
            return UserProfile.Empty;
        }

        var userId = claimsPrincipal.GetUserId() ?? "";
        var userName = claimsPrincipal.GetUserName() ?? "";
        var email = claimsPrincipal.GetEmail() ?? "";
        var roles = claimsPrincipal.GetRoles();

        return new UserProfile(
            UserId: userId,
            UserName: userName,
            Email: email,
            Provider: null, // Not available from claims
            SuperiorName: claimsPrincipal.GetSuperiorName(),
            SuperiorId: claimsPrincipal.GetSuperiorId(),
            ProfilePictureDataUrl: claimsPrincipal.GetProfilePictureDataUrl(),
            DisplayName: claimsPrincipal.GetDisplayName(),
            PhoneNumber: claimsPrincipal.GetPhoneNumber(),
            DefaultRole: roles?.Length > 0 ? roles.First() : Roles.Basic,
            AssignedRoles: roles,
            IsActive: true,
            TenantId: claimsPrincipal.GetTenantId(),
            TenantName: claimsPrincipal.GetTenantName(),
            TimeZoneId: null, // Not available from claims
            LanguageCode: null // Not available from claims
        );
    }

    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Email);
    }

    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
    }

    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
    }

    public static string? GetProvider(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Provider);
    }

    public static string? GetDisplayName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
    }

    public static string? GetProfilePictureDataUrl(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfilePictureDataUrl);
    }

    public static string? GetSuperiorName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorName);
    }

    public static string? GetSuperiorId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorId);
    }

    public static string? GetTenantName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantName);
    }

    public static string? GetTenantId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantId);
    }

    public static bool GetStatus(this ClaimsPrincipal claimsPrincipal)
    {
        return Convert.ToBoolean(claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Status));
    }

    public static string? GetAssignRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.AssignedRoles);
    }

    public static string[] GetRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
    }
} 
