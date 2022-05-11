// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Entities;

public partial class Customer : AuditableEntity, IHasDomainEvent, IAuditTrial,IMayHaveTenant
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public PartnerType PartnerType { get; set; }
    public string? Address { get; set; }
    public string? Contact { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<DomainEvent> DomainEvents { get; set; } = new();
    public string? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
