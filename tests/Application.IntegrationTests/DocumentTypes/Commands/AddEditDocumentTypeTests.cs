using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.IntegrationTests.Documents.Commands
{
    using static Testing;

    public class AddEditDocumentTypeTests : TestBase
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new AddEditDocumentTypeCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateDocumentType()
        {
            var userId = await RunAsDefaultUserAsync();
            var command = new AddEditDocumentTypeCommand()
            {
                Name = "Word",
                Description = "For Test"
            };
            var result = await SendAsync(command);
            
            var item = await FindAsync<DocumentType>(result.Data);

            item.Should().NotBeNull();
            item.Id.Should().Be(result.Data);
            item.Name.Should().Be(command.Name);
            item.CreatedBy.Should().Be(userId);
            item.Created.Should().BeCloseTo(DateTime.Now,new TimeSpan(0,0,10));
            item.LastModifiedBy.Should().BeNull();
            item.LastModified.Should().BeNull();
        }
    }
}
