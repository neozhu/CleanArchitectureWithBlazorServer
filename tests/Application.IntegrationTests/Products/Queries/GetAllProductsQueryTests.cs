using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Queries;

using static Testing;

internal class GetAllProductsQueryTests : TestBase
{
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Product { Name = "Test1" });
        await AddAsync(new Product { Name = "Test2" });
        await AddAsync(new Product { Name = "Test3" });
        await AddAsync(new Product { Name = "Test4" });
    }

    [Test]
    public async Task ShouldQueryAll()
    {
        var query = new GetAllProductsQuery();
        var result = await SendAsync(query);
        Assert.Equals(4, result.Count());
    }

    [Test]
    public async Task ShouldQueryById()
    {
        var query = new GetAllProductsQuery();
        var result = await SendAsync(query);
        var id = result.Last().Id;
        var getProductQuery = new GetProductQuery { Id = id };
        var product = await SendAsync(getProductQuery);
        Assert.Equals(id, product.Id);
    }
}