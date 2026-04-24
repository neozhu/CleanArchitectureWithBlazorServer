using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Queries;

using static Testing;
[NonParallelizable]
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
        Assert.That(result.Count(), Is.EqualTo(4));
    }

    [Test]
    public async Task ShouldQueryById()
    {
        var db = CreateDbContext();
        var id = db.Products.First().Id;
        var getProductQuery = new GetProductQuery { Id = id };
        var product = await SendAsync(getProductQuery);
        Assert.That(product.Id, Is.EqualTo(id));
    }
}
