

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offer");

       builder.Property(x => x.Status).HasMaxLength(255);

        builder.HasOne(o => o.Customer)
               .WithMany(c => c.Offers)
               .HasForeignKey(o => o.CustomerId);

        builder.Ignore(e => e.DomainEvents);
    }
}


