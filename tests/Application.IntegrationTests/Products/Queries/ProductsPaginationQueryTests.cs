using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.IntegrationTests;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.PaginationQuery;
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
        await AddAsync(new Product() { Name = "Test1" });
        await AddAsync(new Product() { Name = "Test2" });
        await AddAsync(new Product() { Name = "Test3" });
        await AddAsync(new Product() { Name = "Test4" });
        await AddAsync(new Product() { Name = "Test5" });
    }
    [Test]
    public async Task ShouldNotEmptyQuery()
    {
        var query = new ProductsWithPaginationQuery();
        var result = await SendAsync(query);
        Assert.AreEqual(5, result.TotalItems);
    }
    [Test]
    public async Task ShouldNotEmptyKewordQuery()
    {
        var query = new ProductsWithPaginationQuery() { Keyword = "1" };
        var result = await SendAsync(query);
        Assert.AreEqual(1, result.TotalItems);
    }
}
