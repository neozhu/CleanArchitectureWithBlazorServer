// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.HasOne(tu => tu.Tenant)
               .WithMany(t => t.TenantUsers)
               .HasForeignKey(tu => tu.TenantId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tu => tu.User).WithMany(x=>x.TenantUsers)
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Cascade);
      
    }
}