// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class LoggerConfiguration : IEntityTypeConfiguration<Logger>
{
    public void Configure(EntityTypeBuilder<Logger> builder)
    {
        builder.Property(x => x.Level).HasMaxLength(450);
        builder.Property(x => x.Message).HasMaxLength(4000);
        builder.Property(x => x.Exception).HasMaxLength(4000);
        builder.Property(x => x.MessageTemplate).HasMaxLength(2000);
        builder.Property(x => x.Properties).HasMaxLength(2000);
        builder.Property(x => x.LogEvent).HasMaxLength(2000);
        builder.HasIndex(x => new { x.Level });
        builder.HasIndex(x => new { x.Message });
        builder.HasIndex(x => new { x.Exception });
        builder.HasIndex(x => x.TimeStamp);
    }
}