#nullable enable

using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Blazor.Application;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using CleanArchitecture.Blazor.Infrastructure;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Infrastructure.MediatorCompatibility;

public class MediatorRuntimeRegistrationTests
{
    [Test]
    public void AddInfrastructure_And_AddApplication_ShouldResolveMediatorRuntime()
    {
        using ServiceProvider provider = CreateServices().BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using IServiceScope scope = provider.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Assert.That(mediator, Is.Not.Null);
    }

    [Test]
    public void AddInfrastructure_And_AddApplication_ShouldUseParallelNoWaitPublisher()
    {
        using ServiceProvider provider = CreateServices().BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using IServiceScope scope = provider.CreateScope();
        INotificationPublisher publisher = scope.ServiceProvider.GetRequiredService<INotificationPublisher>();

        Assert.That(publisher, Is.TypeOf<ParallelNoWaitPublisher>());
    }

    [Test]
    public void AddInfrastructure_And_AddApplication_ShouldRegisterEachPipelineBehaviorOnce()
    {
        var services = CreateServices();

        var duplicatePipelineRegistrations = services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<,>))
            .GroupBy(d => d.ImplementationType)
            .Where(group => group.Key is not null && group.Count() > 1)
            .Select(group => group.Key!.Name)
            .ToList();

        Assert.That(duplicatePipelineRegistrations, Is.Empty);
    }

    private static IServiceCollection CreateServices()
    {
        var settings = new Dictionary<string, string?>
        {
            ["UseInMemoryDatabase"] = bool.TrueString,
            ["DatabaseSettings:DBProvider"] = "sqlite",
            ["DatabaseSettings:ConnectionString"] = "Data Source=:memory:",
            ["AppConfigurationSettings:Secret"] = "unit-test-secret",
            ["AppConfigurationSettings:BehindSSLProxy"] = bool.FalseString,
            ["AppConfigurationSettings:ProxyIP"] = string.Empty,
            ["AppConfigurationSettings:ApplicationUrl"] = "https://localhost",
            ["AppConfigurationSettings:Resilience"] = bool.FalseString,
            ["SmtpClientOptions:Server"] = string.Empty,
            ["SmtpClientOptions:Port"] = "25",
            ["SmtpClientOptions:User"] = string.Empty,
            ["SmtpClientOptions:Password"] = string.Empty,
            ["SmtpClientOptions:UseSsl"] = bool.FalseString,
            ["SmtpClientOptions:RequiresAuthentication"] = bool.FalseString,
            ["SmtpClientOptions:PreferredEncoding"] = string.Empty,
            ["SmtpClientOptions:UsePickupDirectory"] = bool.FalseString,
            ["SmtpClientOptions:MailPickupDirectory"] = string.Empty,
            ["SmtpClientOptions:DefaultFromEmail"] = "noreply@test.local",
            ["IdentitySettings:RequireDigit"] = bool.FalseString,
            ["IdentitySettings:RequiredLength"] = "6",
            ["IdentitySettings:MaxLength"] = "16",
            ["IdentitySettings:RequireNonAlphanumeric"] = bool.FalseString,
            ["IdentitySettings:RequireUpperCase"] = bool.FalseString,
            ["IdentitySettings:RequireLowerCase"] = bool.FalseString,
            ["IdentitySettings:DefaultLockoutTimeSpan"] = "30",
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        services.AddApplication();
        return services;
    }
}
