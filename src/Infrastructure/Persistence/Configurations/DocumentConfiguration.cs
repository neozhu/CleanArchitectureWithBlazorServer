// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.Property(t => t.DocumentType).HasConversion<string>();
        builder.Property(x => x.Content).HasMaxLength(4000);
        builder.Ignore(e => e.DomainEvents);
        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.LastModifier)
            .WithMany()
            .HasForeignKey(x => x.LastModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(e => e.Owner).AutoInclude();
        builder.Navigation(e => e.LastModifier).AutoInclude();
    }
}