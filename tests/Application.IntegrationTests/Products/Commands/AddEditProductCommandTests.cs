using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Commands;

using static Testing;

internal class AddEditProductCommandTests : TestBase
{
    [Test]
    public void ShouldThrowValidationException()
    {
        var command = new AddEditProductCommand();
        FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task InsertItem()
    {
        var addCommand = new AddEditProductCommand
        {
            Name = "Test", Brand = "Brand", Price = 100m, Unit = "EA", Description = "Description",
            Pictures = new List<ProductImage> { new() { Name = "test.jpg", Url = "test.jpg", Size = 1 } }
        };
        var result = await SendAsync(addCommand);
        var find = await FindAsync<Product>(result.Data);
        find.Should().NotBeNull();
        find.Name.Should().Be("Test");
        find.Brand.Should().Be("Brand");
        find.Price.Should().Be(100);
        find.Unit.Should().Be("EA");
    }

    [Test]
    public async Task UpdateItem()
    {
        var addCommand = new AddEditProductCommand
        {
            Name = "Test", Brand = "Brand", Price = 100m, Unit = "EA", Description = "Description",
            Pictures = new List<ProductImage> { new() { Name = "test.jpg", Url = "test.jpg", Size = 1 } }
        };
        var result = await SendAsync(addCommand);
        var find = await FindAsync<Product>(result.Data);
        var editCommand = new AddEditProductCommand
        {
            Id = find.Id, Name = "Test1", Brand = "Brand1", Price = 200m, Unit = "KG", Pictures = addCommand.Pictures,
            Description = "Description1"
        };
        await SendAsync(editCommand);
        var updated = await FindAsync<Product>(find.Id);
        updated.Should().NotBeNull();
        updated.Id.Should().Be(find.Id);
        updated.Name.Should().Be("Test1");
        updated.Brand.Should().Be("Brand1");
        updated.Price.Should().Be(200);
        updated.Unit.Should().Be("KG");
    }
}