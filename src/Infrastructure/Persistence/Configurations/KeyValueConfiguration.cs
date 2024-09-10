// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class KeyConfiguration : IEntityTypeConfiguration<KeyValue>
{
    public void Configure(EntityTypeBuilder<KeyValue> builder)
    {
        builder.Property(t => t.Name).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.Value).HasMaxLength(50);
        builder.Property(t => t.Text).HasMaxLength(100);
        builder.Property(t => t.Description).HasMaxLength(255);
        builder.HasIndex(t => new { t.Name, t.Value }).IsUnique(true);
        builder.Ignore(e => e.DomainEvents);
    }
}