// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.Email);


    public static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);

    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.Name);
    public static string GetSite(this ClaimsPrincipal claimsPrincipal)
      => claimsPrincipal.FindFirstValue(ClaimTypes.Locality);
    public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
         => claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
    public static string GetProfilePictureDataUrl(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfilePictureDataUrl);


    public static string[] GetRoles(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();

    public static void UpdateDisplayName (this ClaimsPrincipal claimsPrincipal, string displayName)
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
    public static void UpdateProfilePictureUrl(this ClaimsPrincipal claimsPrincipal, string url)
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
    public static void UpdatePhoneNumber(this ClaimsPrincipal claimsPrincipal, string phoneNumber)
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
}

