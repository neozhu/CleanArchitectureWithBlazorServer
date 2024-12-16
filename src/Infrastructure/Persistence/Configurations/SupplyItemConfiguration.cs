

//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

//#nullable disable
//public class SupplyItemConfiguration : IEntityTypeConfiguration<SupplyItem>
//{
//    public void Configure(EntityTypeBuilder<SupplyItem> builder)
//    {
//            builder.Property(x => x.Notes).HasMaxLength(255); 
//    builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId); 
//    builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId); 

//        builder.Ignore(e => e.DomainEvents);
//    }
//}


