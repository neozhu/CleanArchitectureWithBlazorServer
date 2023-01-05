// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class KeyConfiguration : IEntityTypeConfiguration<KeyValue>
{
    public void Configure(EntityTypeBuilder<KeyValue> builder)
    {
        builder.Property(t => t.Name).HasConversion<string>();
        builder.Ignore(e => e.DomainEvents);
    }
}
