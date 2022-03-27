
using FluentAssertions;
using System.Threading.Tasks;
using NUnit.Framework;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Application.IntegrationTests.KeyValues.Commands
{
    using static Testing;

    public class DeleteKeyValueTests : TestBase
    {
        [Test]
        public void ShouldRequireValidKeyValueId()
        {
            var command = new DeleteKeyValueCommand (new int[] { 99});

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task ShouldDeleteKeyValue()
        {
            var addcommand = new AddEditKeyValueCommand()
            {
                Name = "Word",
                Text= "Word",
                Value = "Word",
                Description = "For Test"
            };
           var result= await SendAsync(addcommand);

            await SendAsync(new DeleteKeyValueCommand(new int[] { result.Data }));

            var item = await FindAsync<Document>(result.Data);

            item.Should().BeNull();
        }
         
    }
}
