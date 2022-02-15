// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Entities;

public class KeyValue : AuditableEntity, IHasDomainEvent, IAuditTrial
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string Text { get; set; } = default!;
    public string? Description { get; set; }
    public List<DomainEvent> DomainEvents { get; set; } = new();
}
