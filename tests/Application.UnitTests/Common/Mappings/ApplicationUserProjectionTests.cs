using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using FluentAssertions;
using Mapster;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Mappings;

public class ApplicationUserProjectionTests
{
    [Test]
    public async Task ProjectToType_Should_Project_UserTenants_From_RelationalQuery()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var tenant = new Tenant
        {
            Id = "tenant-1",
            Name = "Tenant 1",
            Description = "Primary tenant"
        };

        var user = new ApplicationUser
        {
            Id = "user-1",
            UserName = "alice",
            NormalizedUserName = "ALICE",
            Email = "alice@example.com",
            NormalizedEmail = "ALICE@EXAMPLE.COM",
            IsActive = true,
            TenantId = tenant.Id,
            Tenant = tenant
        };

        var tenantUser = new TenantUser
        {
            Id = "tenant-user-1",
            UserId = user.Id,
            User = user,
            TenantId = tenant.Id,
            Tenant = tenant
        };

        user.TenantUsers.Add(tenantUser);
        tenant.TenantUsers.Add(tenantUser);

        context.Tenants.Add(tenant);
        context.Users.Add(user);
        context.TenantUsers.Add(tenantUser);
        await context.SaveChangesAsync();

        var configuration = MapsterConfiguration.Create();

        var result = await context.Users
            .Where(x => x.Id == user.Id)
            .ProjectToType<ApplicationUserDto>(configuration)
            .FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result!.Tenants.Should().ContainSingle();
        result.Tenants.Single().Id.Should().Be(tenant.Id);
        result.Tenants.Single().Name.Should().Be(tenant.Name);
    }
}
