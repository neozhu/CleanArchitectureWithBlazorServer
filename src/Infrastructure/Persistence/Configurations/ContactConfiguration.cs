// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
        builder.HasOne(t=>t.CreatedBy)
               .WithMany()
               .HasForeignKey(t => t.CreatedById)
               .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(t => t.LastModifiedBy)
               .WithMany()
               .HasForeignKey(t => t.LastModifiedById)
               .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(t => t.CreatedBy).AutoInclude();
        builder.Navigation(t => t.LastModifiedBy).AutoInclude();
        builder.Ignore(e => e.DomainEvents);
    }
}


