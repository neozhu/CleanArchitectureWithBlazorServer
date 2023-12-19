using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.KeyValues.Commands;

using static Testing;

internal class AddEditKeyValueCommandTests : TestBase
{
    [Test]
    public void ShouldThrowValidationException()
    {
        var command = new AddEditKeyValueCommand();
        FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task InsertItem()
    {
        var addCommand = new AddEditKeyValueCommand
            { Name = Picklist.Brand, Text = "Test", Value = "Test", Description = "Description" };
        var result = await SendAsync(addCommand);
        var find = await FindAsync<KeyValue>(result.Data);
        find.Should().NotBeNull();
        find.Name.Should().Be(Picklist.Brand);
        find.Text.Should().Be("Test");
        find.Value.Should().Be("Test");
    }

    [Test]
    public async Task UpdateItem()
    {
        var addCommand = new AddEditKeyValueCommand
            { Name = Picklist.Brand, Text = "Test", Value = "Test", Description = "Description" };
        var result = await SendAsync(addCommand);
        var find = await FindAsync<KeyValue>(result.Data);
        var editCommand = new AddEditKeyValueCommand
            { Id = find.Id, Name = Picklist.Brand, Text = "Test1", Value = "Test1", Description = "Description1" };
        await SendAsync(editCommand);
        var updated = await FindAsync<KeyValue>(find.Id);
        updated.Should().NotBeNull();
        updated.Name.Should().Be(Picklist.Brand);
        updated.Text.Should().Be("Test1");
        updated.Value.Should().Be("Test1");
        updated.Description.Should().Be("Description1");
    }
}