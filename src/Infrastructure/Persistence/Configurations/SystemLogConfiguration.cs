// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
    public void Configure(EntityTypeBuilder<SystemLog> builder)
    {
        builder.Property(x => x.Level).HasMaxLength(450);
        builder.Property(x => x.Message).HasMaxLength(int.MaxValue);
        builder.Property(x => x.Exception).HasMaxLength(int.MaxValue);
        builder.Property(x => x.MessageTemplate).HasMaxLength(int.MaxValue);
        builder.Property(x => x.Properties).HasMaxLength(int.MaxValue);
        builder.Property(x => x.LogEvent).HasMaxLength(int.MaxValue);
        builder.HasIndex(x => new { x.Level });
        builder.HasIndex(x => x.TimeStamp);
       
    }
}
