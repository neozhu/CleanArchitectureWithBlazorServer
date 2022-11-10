// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? "";
    public static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone)??"";
    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
    public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? "";
    public static string GetProvider(this ClaimsPrincipal claimsPrincipal)
      => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Provider) ?? "";
    public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
         => claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? "";
    public static string GetProfilePictureDataUrl(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfilePictureDataUrl)?? "";
    public static string GetSuperiorName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorName)?? "";
    public static string GetSuperiorId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorId)?? "";
   public static string GetTenantName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantName) ?? "";
    public static string GetTenantId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantId) ?? "";
    public static bool GetStatus(this ClaimsPrincipal claimsPrincipal)
       => Convert.ToBoolean(claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Status));
    public static string[] GetRoles(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();

    public static void SetDisplayName (this ClaimsPrincipal claimsPrincipal, string displayName)
    {
        var identity = claimsPrincipal.Identity as ClaimsIdentity;
        if(identity is not null)
        {
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            if (claim is not null)
            {
                identity.RemoveClaim(claim);
            }
            identity.AddClaim(new Claim( ClaimTypes.GivenName, displayName ));
        }
        
    }
    public static void SetProfilePictureUrl(this ClaimsPrincipal claimsPrincipal, string url)
    {
        var identity = claimsPrincipal.Identity as ClaimsIdentity;
        if (identity is not null)
        {
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ApplicationClaimTypes.ProfilePictureDataUrl);
            if (claim is not null)
            {
                identity.RemoveClaim(claim);
            }
            identity.AddClaim(new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, url));
        }
    }
    public static void SetPhoneNumber(this ClaimsPrincipal claimsPrincipal, string phoneNumber)
    {
        var identity = claimsPrincipal.Identity as ClaimsIdentity;
        if (identity is not null)
        {
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone);
            if (claim is not null)
            {
                identity.RemoveClaim(claim);
            }
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, phoneNumber));
        }
    }
    public static void SetProvider(this ClaimsPrincipal claimsPrincipal, string provider)
    {
        var identity = claimsPrincipal.Identity as ClaimsIdentity;
        if (identity is not null)
        {
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ApplicationClaimTypes.Provider);
            if (claim is not null)
            {
                identity.RemoveClaim(claim);
            }
            identity.AddClaim(new Claim(ApplicationClaimTypes.Provider, provider));
        }
    }
    public static void SetTenantId(this ClaimsPrincipal claimsPrincipal, string tenantId)
    {
        var identity = claimsPrincipal.Identity as ClaimsIdentity;
        if (identity is not null)
        {
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ApplicationClaimTypes.TenantId);
            if (claim is not null)
            {
                identity.RemoveClaim(claim);
            }
            identity.AddClaim(new Claim(ApplicationClaimTypes.TenantId, tenantId));
        }
    }
    public static void SetTenantName(this ClaimsPrincipal claimsPrincipal, string tenantName)
    {
        var identity = claimsPrincipal.Identity as ClaimsIdentity;
        if (identity is not null)
        {
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ApplicationClaimTypes.TenantName);
            if (claim is not null)
            {
                identity.RemoveClaim(claim);
            }
            identity.AddClaim(new Claim(ApplicationClaimTypes.TenantName, tenantName));
        }
    }
}

