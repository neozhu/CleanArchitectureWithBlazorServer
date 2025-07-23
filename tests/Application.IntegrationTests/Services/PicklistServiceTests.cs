using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Services;

using static Testing;
[NonParallelizable]
public class PicklistServiceTests : TestBase
{
  
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new PicklistSet { Name = Picklist.Brand, Text = "Text1", Value = "Value1" });
        await AddAsync(new PicklistSet { Name = Picklist.Brand, Text = "Text2", Value = "Value2" });
        await AddAsync(new PicklistSet { Name = Picklist.Brand, Text = "Text3", Value = "Value3" });
        await AddAsync(new PicklistSet { Name = Picklist.Brand, Text = "Text4", Value = "Value4" });
    }

    [Test]
    public async Task ShouldLoadDataSource()
    {
        var picklist = CreatePicklistService();
        await picklist.RefreshAsync();
        var count = picklist.DataSource.Count();
        Assert.That(count,Is.EqualTo(4));
  
    }

    [Test]
    public async Task ShouldUpdateDataSource()
    {
        await AddAsync(new PicklistSet { Name = Picklist.Brand, Text = "Text5", Value = "Value5" });
        var picklist = CreatePicklistService();
        await picklist.RefreshAsync();
        var count = picklist.DataSource.Count();
        Assert.That(count, Is.EqualTo(5));
    }
}