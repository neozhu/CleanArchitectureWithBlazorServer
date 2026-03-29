using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;
using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Common.MediatorCompatibility;

using static Testing;

public class MediatorCompatibilityTests : TestBase
{
    [Test]
    public async Task Should_resolve_mediator_from_di_and_send_command()
    {
        var product = new Product { Name = "Compatibility product" };
        await AddAsync(product);

        using IServiceScope scope = CreateScope();

        Mediator.IMediator mediator = scope.ServiceProvider.GetRequiredService<Mediator.IMediator>();
        var result = await mediator.Send(new DeleteProductCommand([product.Id]));

        Assert.That(result.Succeeded, Is.True);
        Assert.That(await CountAsync<Product>(), Is.EqualTo(0));
    }

    [Test]
    public async Task Should_resolve_scoped_mediator_from_di_and_send_command()
    {
        var product = new Product { Name = "Scoped compatibility product" };
        await AddAsync(product);

        using IServiceScope scope = CreateScope();

        var scopedMediator = scope.ServiceProvider.GetRequiredService<CleanArchitecture.Blazor.Application.Common.Interfaces.MediatorWrapper.IScopedMediator>();
        var result = await scopedMediator.Send(new DeleteProductCommand([product.Id]));

        Assert.That(result.Succeeded, Is.True);
        Assert.That(await CountAsync<Product>(), Is.EqualTo(0));
    }
}
