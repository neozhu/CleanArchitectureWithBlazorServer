using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.IntegrationTests;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.Export;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.PaginationQuery;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.KeyValues.Queries;
using static Testing;
internal class KeyValuesWithPaginationQueryTests: TestBase
{
    [SetUp]
    public async Task initData()
    {
        await AddAsync<KeyValue>(new KeyValue() { Name = Domain.Picklist.Brand, Text = "Text1", Value = "Value1" ,Description= "Test Description" });
        await AddAsync<KeyValue>(new KeyValue() { Name = Domain.Picklist.Brand, Text = "Text2", Value = "Value2", Description = "Test Description" });
        await AddAsync<KeyValue>(new KeyValue() { Name = Domain.Picklist.Brand, Text = "Text3", Value = "Value3", Description = "Test Description" });
        await AddAsync<KeyValue>(new KeyValue() { Name = Domain.Picklist.Brand, Text = "Text4", Value = "Value4", Description = "Test Description" });
        await AddAsync<KeyValue>(new KeyValue() { Name = Domain.Picklist.Brand, Text = "Text5", Value = "Value5", Description = "Test Description" });

    }
    [Test]
    public async Task ShouldNotEmptyQuery()
    {
        var query = new KeyValuesWithPaginationQuery();
        var result = await SendAsync(query);
        Assert.AreEqual(5, result.TotalItems);
    }
    [Test]
    public async Task ShouldNotEmptyKewordQuery()
    {
        var query = new KeyValuesWithPaginationQuery() { Keyword="1"};
        var result = await SendAsync(query);
        Assert.AreEqual(1, result.TotalItems);
    }
}
