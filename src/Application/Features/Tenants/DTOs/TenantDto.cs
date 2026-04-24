// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

[Description("Tenants")]
public class TenantDto : IEquatable<TenantDto>
{
    [Display(Name = "Tenant Id")] public string Id { get; set; } = Guid.CreateVersion7().ToString();
    [Display(Name = "Tenant Name")] public string? Name { get; set; }
    [Display(Name = "Description")] public string? Description { get; set; }    public bool Equals(TenantDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }
    public override bool Equals(object? obj) => obj is TenantDto state && Equals(state);

    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString() => Name ?? string.Empty;
}
