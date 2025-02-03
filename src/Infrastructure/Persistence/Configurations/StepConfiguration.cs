
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class StepConfiguration : IEntityTypeConfiguration<Step>
{
    public void Configure(EntityTypeBuilder<Step> builder)
    {
        // Mapping Step entity to Steps table
        builder.ToTable("Steps");

        // Defining primary key
        builder.HasKey(s => s.Id);

        // Configuring Name property
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(255);

        // Configuring IsCompleted property
        builder.Property(s => s.IsCompleted)
            .IsRequired();

        // Configuring StepOrder property
        builder.Property(s => s.StepOrder)
            .IsRequired();

        builder.OwnsMany(s => s.Comments, c =>
        {
            // Ensuring Comments reference Step
            c.WithOwner().HasForeignKey(c => c.StepId);

            // Configuring Content property
            c.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000);

        });

    }
}