using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.IntegrationTests;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.Export;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Export;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Queries;
using static Testing;
internal class ExportProductsQueryTests : TestBase
{
    [Test]
    public async Task ShouldNotEmptyExportQuery()
    {
        var query = new ExportProductsQuery();
        var result = await SendAsync(query);
        result.Should().NotBeNull();
    }

    [Test]
    public async Task ShouldNotEmptyExportQueryWithFilter()
    {
        var query = new ExportProductsQuery() { Keyword="1" };
        var result = await SendAsync(query);
        result.Should().NotBeNull();
    }
}
