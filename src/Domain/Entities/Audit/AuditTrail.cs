// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitecture.Razor.Domain.Entities.Audit;

public class AuditTrail : IEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public AuditType AuditType { get; set; }
    public string TableName { get; set; }
    public DateTime DateTime { get; set; }
    public Dictionary<string, object> OldValues { get; set; } = new();
    public Dictionary<string, object> NewValues { get; set; } = new();
    public ICollection<string> AffectedColumns { get; set; }
    public Dictionary<string, object> PrimaryKey { get; set; } = new();

    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public bool HasTemporaryProperties => TemporaryProperties.Any();
}
