using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.IntegrationTests;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Commands;
using static Testing;
internal class DeleteProductCommandTests: TestBase
{
    [Test]
        public void ShouldRequireValidKeyValueId()
    {
        var command = new DeleteProductCommand(new int[] { 99 });

        FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteOne()
    {
        var addcommand = new AddEditProductCommand() { Name = "Test", Brand = "Brand", Price = 100m, Unit = "EA", Description = "Description" };
        var result = await SendAsync(addcommand);

        await SendAsync(new DeleteProductCommand(new int[] { result.Data }));

        var item = await FindAsync<Product>(result.Data);

        item.Should().BeNull();
    }
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Product() { Name = "Test1" });
        await AddAsync(new Product() { Name = "Test2" });
        await AddAsync(new Product() { Name = "Test3" });
        await AddAsync(new Product() { Name = "Test4" });
    }
    [Test]
    public async Task ShouldDeleteAll()
    {
        var query = new GetAllProductsQuery();
        var result = await SendAsync(query);
        result.Count().Should().Be(4);
        var id = result.Select(x => x.Id).ToArray();
        var deleted = await SendAsync(new DeleteProductCommand(id));
        deleted.Succeeded.Should().BeTrue();

        var deleteresult = await SendAsync(query);
        deleteresult.Should().BeNullOrEmpty();


    }
}
