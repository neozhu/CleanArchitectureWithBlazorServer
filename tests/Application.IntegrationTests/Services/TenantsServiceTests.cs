﻿using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Services;

using static Testing;

public class TenantsServiceTests : TestBase
{
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Tenant { Name = "Test1", Description = "Test1" });
        await AddAsync(new Tenant { Name = "Test2", Description = "Text2" });
    }

    [Test]
    public async Task ShouldLoadDataSource()
    {
        var tenantsService = CreateTenantsService();
        await tenantsService.InitializeAsync();
        var count = tenantsService.DataSource.Count();
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task ShouldUpdateDataSource()
    {
        await AddAsync(new Tenant { Name = "Test3", Description = "Test3" });
        var tenantsService = CreateTenantsService();
        await tenantsService.RefreshAsync();
        var count = tenantsService.DataSource.Count();
        Assert.That(count, Is.EqualTo(3));
    }
}
