using System;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Queries;

using static Testing;

internal class ProductsPaginationQueryTests : TestBase
{
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Product { Name = "Test1", Price = 19, Brand = "Test1", Unit = "EA", Description = "Test1" });
        await AddAsync(new Product { Name = "Test2", Price = 19, Brand = "Test2", Unit = "EA", Description = "Test1" });
        await AddAsync(new Product { Name = "Test3", Price = 19, Brand = "Test3", Unit = "EA", Description = "Test1" });
        await AddAsync(new Product { Name = "Test4", Price = 19, Brand = "Test4", Unit = "EA", Description = "Test1" });
        await AddAsync(new Product { Name = "Test5", Price = 19, Brand = "Test5", Unit = "EA", Description = "Test1" });
    }

    [Test]
    public async Task ShouldNotEmptyQuery()
    {
        var query = new ProductsWithPaginationQuery(){ CurrentUser = new Common.Security.UserProfile(Guid.NewGuid().ToString(), "test", "test@test.com", TimeZoneId: "Asia/Shanghai") };
        var result = await SendAsync(query);
        Assert.That(result.TotalItems, Is.EqualTo(5));
    }

    [Test]
    public async Task ShouldNotEmptyKeywordQuery()
    {
        var query = new ProductsWithPaginationQuery { Keyword = "1" , CurrentUser = new Common.Security.UserProfile(Guid.NewGuid().ToString(), "test", "test@test.com", TimeZoneId: "Asia/Shanghai") };
        var result = await SendAsync(query);
        Assert.That(result.TotalItems, Is.EqualTo(5));
    }

    [Test]
    public async Task ShouldNotEmptySpecificationQuery()
    {
        var query = new ProductsWithPaginationQuery { Keyword = "1", Brand = "Test1", Unit = "EA", Name = "Test1" , CurrentUser = new Common.Security.UserProfile(Guid.NewGuid().ToString(), "test", "test@test.com", TimeZoneId: "Asia/Shanghai") };
        var result = await SendAsync(query);
        Assert.That(result.TotalItems, Is.EqualTo(1));
    }
}
