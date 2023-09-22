// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Reflection.Emit;
using CleanArchitecture.Blazor.Domain.Entities.Logger;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

#nullable disable
public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, string,
    ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
    ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options)
    {

    }
    // public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }


    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Logger> Loggers { get; set; }
    public DbSet<AuditTrail> AuditTrails { get; set; }
    public DbSet<Document> Documents { get; set; }

    public DbSet<KeyValue> KeyValues { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync(CancellationToken.None);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);

        builder.Entity<ApplicationUserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId, ur.TenantId });

        /*
        builder.Entity<ApplicationRole>()
             .HasKey(r => new { r.Id, r.TenantType });

        builder.Entity<ApplicationRoleClaim>()
    .HasOne<ApplicationRole>()
    .WithMany()
    .HasForeignKey(rc => rc.RoleId);*/
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine);
        if (!optionsBuilder.IsConfigured)
        {

        }
    }

}