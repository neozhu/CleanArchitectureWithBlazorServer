using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.IntegrationTests;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.Export;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.KeyValues.Queries;
using static Testing;
internal class ExportKeyValuesQueryTests : TestBase
{
 

    [Test]
    public async Task ShouldNotEmptyExportQuery()
    {
        var query = new ExportKeyValuesQuery();
        var result =await SendAsync(query);
        result.Should().NotBeNull();
    }
}
