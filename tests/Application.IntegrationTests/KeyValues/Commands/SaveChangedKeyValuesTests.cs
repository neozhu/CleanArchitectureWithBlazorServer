using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.SaveChanged;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Application.IntegrationTests.KeyValues.Commands
{
    using static Testing;

    public class SaveChangedKeyValuesTests : TestBase
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new SaveChangedKeyValuesCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateDocumentType()
        {
            var userId = await RunAsDefaultUserAsync();
            var command = new SaveChangedKeyValuesCommand()
            {
                Items = new KeyValueDto[] {
                    new KeyValueDto(){ Name="Name",Value="value1",Text="text1", TrackingState= TrackingState.Added },
                    new KeyValueDto(){ Name="Name",Value="value2",Text="text2", TrackingState= TrackingState.Added },
                    new KeyValueDto(){ Name="Name",Value="value3",Text="text3", TrackingState= TrackingState.Added }
                    }
            };
            var result = await SendAsync(command);
            
            var count = await CountAsync<KeyValue>();

            result.Succeeded.Should().BeTrue();
            count.Should().Be(3);
        }
    }
}
