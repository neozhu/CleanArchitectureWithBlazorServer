// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

[Description("Tenants")]
public class TenantDto : IEquatable<TenantDto>
{
    [Description("Tenant Id")] public string Id { get; set; } = Guid.CreateVersion7().ToString();
    [Description("Tenant Name")] public string? Name { get; set; }
    [Description("Description")] public string? Description { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Tenant, TenantDto>().ReverseMap();
        }
    }
    public bool Equals(TenantDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }
    public override bool Equals(object? obj) => obj is TenantDto state && Equals(state);

    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString() => Name ?? string.Empty;
}
