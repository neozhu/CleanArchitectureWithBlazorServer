

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offer");

        builder.Property(x => x.Status).HasMaxLength(255);

        builder.HasOne(x => x.Customer);

        builder.OwnsMany(o => o.OfferLines, x =>
        {
            x.ToTable("OfferLine"); // Map to separate table

            x.WithOwner().HasForeignKey("OfferId"); // Foreign key

            x.HasOne(ol => ol.Product)
               .WithMany()
               .HasForeignKey(x => x.ItemId);
        });

        builder.Ignore(e => e.DomainEvents);
    }
}


