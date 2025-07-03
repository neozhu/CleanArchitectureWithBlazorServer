// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class UserLoginRiskSummaryConfiguration : IEntityTypeConfiguration<UserLoginRiskSummary>
{
    public void Configure(EntityTypeBuilder<UserLoginRiskSummary> builder)
    {
        builder.Property(x => x.Id).HasMaxLength(36);
        builder.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.UserName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.RiskLevel).HasConversion<string>();
        builder.Property(x => x.RiskScore).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Advice).HasMaxLength(1000);

        // Add index for frequently queried fields
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.UserName);

        
        // Configure relationship with ApplicationUser
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
