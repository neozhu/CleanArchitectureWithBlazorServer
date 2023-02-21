using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.IntegrationTests;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.Import;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Commands;

using static Testing;
internal class ImportProductsCommandTests: TestBase
{
    [Test]
    public async Task DownloadTemplate()
    {
        var cmd = new CreateProductsTemplateCommand();
        var result = await SendAsync(cmd);
        result.Succeeded.Should().BeTrue();
    }

    [Test]
    public async Task ImportDataFromExcel()
    {
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var excelfile = Path.Combine(dir,"../../../", "Products", "ImportExcel", "Products.xlsx");
        var data = File.ReadAllBytes(excelfile);
        var cmd = new ImportProductsCommand("Products.xlsx", data);
        var result = await SendAsync(cmd);
        result.Succeeded.Should().BeTrue();
    }
}
