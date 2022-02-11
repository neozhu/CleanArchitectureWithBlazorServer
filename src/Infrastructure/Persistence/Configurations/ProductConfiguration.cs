// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(e => e.Pictures)
           .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                 v => JsonSerializer.Deserialize<IList<string>>(v, (JsonSerializerOptions)null),
                 new ValueComparer<IList<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                               c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                               c => c.ToList()));

        
    }
}
