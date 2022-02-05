using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Features.Customers.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.IntegrationTests.Customers.Commands
{
    using static Testing;

    public class AddEditCustomerTests : TestBase
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new AddEditCustomerCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateCustomer()
        {
            var userId = await RunAsDefaultUserAsync();
            var command = new AddEditCustomerCommand
            {
                Name = "Name",
                NameOfEnglish = "NameOfEnglish",
                GroupName = "GroupName",
                Region = "Region",
                Sales = "Sales",
                RegionSalesDirector = "RegionSalesDirector",
                PartnerType = "IC"
            };
            var result = await SendAsync(command);

            var item = await FindAsync<Customer>(result.Data);

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
