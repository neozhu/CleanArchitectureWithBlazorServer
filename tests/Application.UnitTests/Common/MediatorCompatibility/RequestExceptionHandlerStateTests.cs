using System.Linq;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.MediatorCompatibility;

public class MediatorAbstractionsSmokeTests
{
    [Test]
    public void Contracts_ShouldCompileAgainstMediatorAbstractions()
    {
        _ = new SmokeNotification();
        _ = new SmokeRequest();
        _ = typeof(IPipelineBehavior<SmokeRequest, string>);
    }

    [Test]
    public void AddApplication_ShouldNotRegisterMediatorPipelineBehaviorsDirectly()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        var pipelineImplementations = services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<,>))
            .Select(d => d.ImplementationType)
            .ToList();

        Assert.That(pipelineImplementations, Is.Empty);
    }

    private sealed record SmokeNotification : INotification;

    private sealed record SmokeRequest : IRequest<string>;
}
