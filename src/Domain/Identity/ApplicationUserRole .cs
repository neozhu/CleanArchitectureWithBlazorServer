// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationUserRole : IdentityUserRole<string>
{
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ApplicationRole Role { get; set; } = default!;

    [ForeignKey("TenantId")]
    public virtual Tenant Tenant { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
