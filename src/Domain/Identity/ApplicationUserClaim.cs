// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationUserClaim : IdentityUserClaim<string>
{
    public string? Description { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
}