
using FluentAssertions;
using System.Threading.Tasks;
using NUnit.Framework;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.Delete;

namespace CleanArchitecture.Application.IntegrationTests.Documents.Commands
{
    using static Testing;

    public class DeleteDocumentTypeTests : TestBase
    {
        [Test]
        public void ShouldRequireValidDocumentTypeId()
        {
            var command = new DeleteDocumentTypeCommand(new int[] { 1});

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task ShouldDeleteDocumentType()
        {
            var addcommand = new AddEditDocumentTypeCommand()
            {
                Name = "Word",
                Description = "For Test"
            };
           var result= await SendAsync(addcommand);
             
            await SendAsync(new DeleteDocumentTypeCommand(new int[] { result.Data }));

            var item = await FindAsync<Document>(result.Data);

            item.Should().BeNull();
        }
         
    }
}
