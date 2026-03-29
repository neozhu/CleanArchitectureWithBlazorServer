using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll;
using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Common.MediatorCompatibility;

using static Testing;

public class MediatorCompatibilityTests : TestBase
{
    [Test]
    public async Task Should_resolve_compatibility_mediators_from_di_and_send_queries()
    {
        await AddAsync(new Product { Name = "Compatibility product" });

        using IServiceScope scope = CreateScope();

        Mediator.IMediator mediator = scope.ServiceProvider.GetRequiredService<Mediator.IMediator>();
        var scopedMediator = scope.ServiceProvider.GetRequiredService<CleanArchitecture.Blazor.Application.Common.Interfaces.MediatorWrapper.IScopedMediator>();

        var products = (await mediator.Send(new GetAllProductsQuery())).ToList();
        var scopedProducts = (await scopedMediator.Send(new GetAllProductsQuery())).ToList();

        Assert.That(products, Has.Count.EqualTo(1));
        Assert.That(scopedProducts, Has.Count.EqualTo(1));
    }
}
