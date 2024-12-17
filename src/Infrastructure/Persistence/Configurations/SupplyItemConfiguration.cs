
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class SupplyItemConfiguration : IEntityTypeConfiguration<SupplyItem>
{
    public void Configure(EntityTypeBuilder<SupplyItem> builder)
    {
        builder.ToTable("SupplyItems");

        // Property Configurations
        builder.Property(x => x.Notes)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.SupplierId)
            .IsRequired();

        builder.HasOne(d => d.Product).WithMany(p => p.SupplyItems)
             .HasForeignKey(d => d.ProductId)
             .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.Supplier).WithMany(p => p.SupplyItems)
            .HasForeignKey(d => d.SupplierId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Ignore(x => x.DomainEvents);
    }
}


