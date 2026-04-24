#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Domain.Identity;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.Notifications;

[TestFixture]
public class RemainingNotificationMediatorSmokeTests
{
    [Test]
    public async Task Publish_ShouldReach_ProductUpdatedEventHandler()
    {
        var notification = new ProductUpdatedEvent(new Product { Id = 7, Name = "Updated Product" });
        var message = await PublishAndCaptureAsync(
            "CleanArchitecture.Blazor.Application.Features.Products.EventHandlers.ProductUpdatedEventHandler",
            "ProductUpdatedEvent",
            notification);

        Assert.That(message, Does.Contain("ProductUpdatedEvent"));
    }

    [Test]
    public async Task Publish_ShouldReach_DocumentDeletedEventHandler()
    {
        var notification = new DocumentDeletedEvent(new Document { Id = 9, URL = string.Empty });
        var message = await PublishAndCaptureAsync(
            "CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers.DocumentDeletedEventHandler",
            "skipping file deletion",
            notification);

        Assert.That(message, Does.Contain("skipping file deletion"));
    }

    [Test]
    public async Task Publish_ShouldReach_PicklistSetChangedEventHandler()
    {
        var refreshed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var dataSource = new Mock<IDataSourceService<PicklistSetDto>>();
        dataSource.SetupGet(x => x.DataSource).Returns(Array.Empty<PicklistSetDto>());
        dataSource.Setup(x => x.RefreshAsync())
            .Returns(Task.CompletedTask)
            .Callback(() => refreshed.TrySetResult(true));

        await PublishAndAwaitAsync(
            new PicklistSetUpdatedEvent(new PicklistSet { Id = 11 }),
            services => services.AddSingleton(dataSource.Object));

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await refreshed.Task.WaitAsync(timeout.Token);
        dataSource.Verify(x => x.RefreshAsync(), Times.Once);
    }

     

    private static async Task<string> PublishAndCaptureAsync<TNotification>(
        string categoryName,
        string eventName,
        TNotification notification)
        where TNotification : INotification
    {
        using var logProvider = new TestLogProvider(categoryName, eventName);
        await PublishAndAwaitAsync(
            notification,
            services => services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(logProvider);
            }));

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        return await logProvider.WaitForMatchAsync(timeout.Token);
    }

    private static async Task PublishAndAwaitAsync<TNotification>(
        TNotification notification,
        Action<IServiceCollection>? configureServices = null)
        where TNotification : INotification
    {
        await using var provider = BuildProvider(configureServices);
        await using var scope = provider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(notification, CancellationToken.None);
    }

    private static ServiceProvider BuildProvider(Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        configureServices?.Invoke(services);

        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(CleanArchitecture.Blazor.Application.DependencyInjection)];
            options.NotificationPublisherType = typeof(ChannelBasedNoWaitPublisher);
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });

        return services.BuildServiceProvider();
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
