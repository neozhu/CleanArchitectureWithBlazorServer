using System.Collections.Generic;
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
        using ServiceProvider provider = CreateServices().BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        });

        IMediator mediator = provider.GetRequiredService<IMediator>();

        Assert.That(mediator, Is.Not.Null);
    }

    [Test]
    public void AddInfrastructure_And_AddApplication_ShouldUseParallelNoWaitPublisher()
    {
        using ServiceProvider provider = CreateServices().BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        });

        INotificationPublisher publisher = provider.GetRequiredService<INotificationPublisher>();

        Assert.That(publisher, Is.TypeOf<ParallelNoWaitPublisher>());
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
