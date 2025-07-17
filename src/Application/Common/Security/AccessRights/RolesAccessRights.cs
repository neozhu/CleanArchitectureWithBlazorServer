// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class RolesAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
    public bool ManagePermissions { get; set; }
    public bool ManageClaimsInRole { get; set; }
    public bool ManageUsersInRole { get; set; }
    public bool ViewPermissions { get; set; }
    public bool ViewClaimsInRole { get; set; }
    public bool ViewUsersInRole { get; set; }
} 