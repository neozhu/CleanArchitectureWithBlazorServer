

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(x => x.Status).HasMaxLength(255); 

        builder.Ignore(e => e.DomainEvents);

        builder.OwnsMany(o => o.InvoiceLines, x =>
        {
            x.ToTable("InvoiceLines");             // Map to separate table

            x.WithOwner().HasForeignKey("InvoiceId"); // Foreign key

            x.HasOne(ol => ol.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId);

        });

    }
}


