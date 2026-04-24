#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.Contacts.EventHandlers;

[TestFixture]
public class ContactUpdatedEventHandlerMediatorSmokeTests
{
    [Test]
    public async Task Publish_ShouldReach_ContactCreatedEventHandler()
    {
        var notification = new ContactCreatedEvent(new Contact { Name = "Created" });
        var message = await PublishAndCaptureAsync(
            "CleanArchitecture.Blazor.Application.Features.Contacts.EventHandlers.ContactCreatedEventHandler",
            "ContactCreatedEvent",
            notification);

        Assert.That(message, Does.Contain("ContactCreatedEvent"));
    }

    [Test]
    public async Task Publish_ShouldReach_ContactUpdatedEventHandler()
    {
        var notification = new ContactUpdatedEvent(new Contact { Name = "Updated" });
        var message = await PublishAndCaptureAsync(
            "CleanArchitecture.Blazor.Application.Features.Contacts.EventHandlers.ContactUpdatedEventHandler",
            "ContactUpdatedEvent",
            notification);

        Assert.That(message, Does.Contain("ContactUpdatedEvent"));
    }

    [Test]
    public async Task Publish_ShouldReach_ContactDeletedEventHandler()
    {
        var notification = new ContactDeletedEvent(new Contact { Name = "Deleted" });
        var message = await PublishAndCaptureAsync(
            "CleanArchitecture.Blazor.Application.Features.Contacts.EventHandlers.ContactDeletedEventHandler",
            "ContactDeletedEvent",
            notification);

        Assert.That(message, Does.Contain("ContactDeletedEvent"));
    }

    private static async Task<string> PublishAndCaptureAsync<TNotification>(
        string categoryName,
        string eventName,
        TNotification notification)
        where TNotification : INotification
    {
        using var logProvider = new TestLogProvider(
            categoryName,
            eventName);

        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(logProvider);
        });

        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(CleanArchitecture.Blazor.Application.DependencyInjection)];
            options.NotificationPublisherType = typeof(ChannelBasedNoWaitPublisher);
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(notification, CancellationToken.None);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        return await logProvider.WaitForMatchAsync(timeout.Token);
    }

    private sealed class TestLogProvider : ILoggerProvider
    {
        private readonly string _targetCategory;
        private readonly string _targetText;
        private readonly TaskCompletionSource<string> _matchedMessage =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TestLogProvider(string targetCategory, string targetText)
        {
            _targetCategory = targetCategory;
            _targetText = targetText;
        }

        public ILogger CreateLogger(string categoryName) => new TestLogger(categoryName, this);

        public Task<string> WaitForMatchAsync(CancellationToken cancellationToken) =>
            _matchedMessage.Task.WaitAsync(cancellationToken);

        public void Dispose()
        {
        }

        private void TryMatch(string categoryName, string message)
        {
            if (categoryName == _targetCategory && message.Contains(_targetText, StringComparison.Ordinal))
            {
                _matchedMessage.TrySetResult(message);
            }
        }

        private sealed class TestLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly TestLogProvider _provider;

            public TestLogger(string categoryName, TestLogProvider provider)
            {
                _categoryName = categoryName;
                _provider = provider;
            }

            public IDisposable BeginScope<TState>(TState state)
                where TState : notnull
            {
                return NullScope.Instance;
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                _provider.TryMatch(_categoryName, formatter(state, exception));
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
#nullable restore
