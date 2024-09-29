using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.Delete;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.KeyValues.Commands;

using static Testing;

public class DeleteKeyValueTests : TestBase
{
    [Test]
    public void ShouldRequireValidKeyValueId()
    {
        var command = new DeletePicklistSetCommand(new[] { 99 });

        FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteKeyValue()
    {
        var addCommand = new AddEditPicklistSetCommand
        {
            Name = Picklist.Brand,
            Text = "Word",
            Value = "Word",
            Description = "For Test"
        };
        var result = await SendAsync(addCommand);

        await SendAsync(new DeletePicklistSetCommand(new[] { result.Data }));

        var item = await FindAsync<Document>(result.Data);

        item.Should().BeNull();
    }
}