
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasIndex(x => x.Name); 
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Comments).HasMaxLength(500).IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}


