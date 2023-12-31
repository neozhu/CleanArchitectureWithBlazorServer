// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Navigation(e => e.Owner).AutoInclude();
        builder.Property(t => t.AuditType)
            .HasConversion<string>();
        builder.Property(e => e.AffectedColumns).HasStringListConversion();
        builder.Property(u => u.OldValues).HasJsonConversion();
        builder.Property(u => u.NewValues).HasJsonConversion();
        builder.Property(u => u.PrimaryKey).HasJsonConversion();
        builder.Ignore(x => x.TemporaryProperties);
        builder.Ignore(x => x.HasTemporaryProperties);
    }
}