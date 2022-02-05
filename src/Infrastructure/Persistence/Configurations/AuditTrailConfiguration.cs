// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using CleanArchitecture.Razor.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.Property(t => t.AuditType)
           .HasConversion<string>();
        builder.Property(e => e.AffectedColumns)
           .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                 v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                 new ValueComparer<ICollection<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                               c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                               c => (ICollection<string>)c.ToList()));

        builder.Property(u => u.OldValues)
            .HasConversion(
                d => JsonSerializer.Serialize(d, (JsonSerializerOptions)null),
                s => JsonSerializer.Deserialize<Dictionary<string, object>>(s, (JsonSerializerOptions)null)
            );
        builder.Property(u => u.NewValues)
            .HasConversion(
                d => JsonSerializer.Serialize(d, (JsonSerializerOptions)null),
                s => JsonSerializer.Deserialize<Dictionary<string, object>>(s, (JsonSerializerOptions)null)
            );
        builder.Property(u => u.PrimaryKey)
            .HasConversion(
                d => JsonSerializer.Serialize(d, (JsonSerializerOptions)null),
                s => JsonSerializer.Deserialize<Dictionary<string, object>>(s, (JsonSerializerOptions)null)
            );

        builder.Ignore(x => x.TemporaryProperties);
        builder.Ignore(x => x.HasTemporaryProperties);
    }
}
