using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using Mediator;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.PublishStrategies;

[TestFixture]
public class ChannelBasedNoWaitPublisherTests
{
    [SetUp]
    public void SetUp()
    {
        SlowNotificationHandler.Reset();
        ThrowingNotificationHandler.Reset();
        BlockingNotificationHandler.Reset();
        ObservingNotificationHandler.Reset();
    }

    [Test]
    public async Task Publish_ShouldReturnBeforeSlowHandlerCompletes()
    {
        await using var publisher = CreatePublisher();
        INotificationHandler<SlowNotification>[] handlerArray = [new SlowNotificationHandler()];
        var handlers = new NotificationHandlers<SlowNotification>(handlerArray, isArray: true);

        var stopwatch = Stopwatch.StartNew();
        await publisher.Publish(handlers, new SlowNotification(), CancellationToken.None);
        stopwatch.Stop();

        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), "publish should not wait for slow notification handlers");

        using var completed = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await SlowNotificationHandler.Completed.Task.WaitAsync(completed.Token);
        Assert.That(SlowNotificationHandler.ExecutionCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Publish_ShouldNotBubbleHandlerExceptions()
    {
        await using var publisher = CreatePublisher();
        INotificationHandler<ThrowingNotification>[] handlerArray = [new ThrowingNotificationHandler()];
        var handlers = new NotificationHandlers<ThrowingNotification>(handlerArray, isArray: true);

        Assert.DoesNotThrowAsync(async () => await publisher.Publish(handlers, new ThrowingNotification(), CancellationToken.None));
    }

    [Test]
    public async Task DisposeAsync_ShouldDrainQueuedHandlers()
    {
        await using var publisher = CreatePublisher();
        INotificationHandler<DrainNotification>[] handlerArray =
        [
            new BlockingNotificationHandler(),
            new ObservingNotificationHandler()
        ];
        var handlers = new NotificationHandlers<DrainNotification>(handlerArray, isArray: true);

        await publisher.Publish(handlers, new DrainNotification(), CancellationToken.None);

        var disposeTask = publisher.DisposeAsync().AsTask();

        using var started = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await BlockingNotificationHandler.Started.Task.WaitAsync(started.Token);
        BlockingNotificationHandler.Release.TrySetResult(true);

        using var completed = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await disposeTask.WaitAsync(completed.Token);

        Assert.That(ObservingNotificationHandler.ExecutionCount, Is.EqualTo(1),
            "disposing the publisher should not drop queued handlers");
    }

    private static ChannelBasedNoWaitPublisher CreatePublisher()
    {
        return new ChannelBasedNoWaitPublisher(NullLogger<ChannelBasedNoWaitPublisher>.Instance);
    }

    public sealed record SlowNotification : INotification;

    public sealed class SlowNotificationHandler : INotificationHandler<SlowNotification>
    {
        private static int _executionCount;
        public static TaskCompletionSource<bool> Completed { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static int ExecutionCount => _executionCount;

        public static void Reset()
        {
            Completed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _executionCount = 0;
        }

        public async ValueTask Handle(SlowNotification notification, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _executionCount);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            Completed.TrySetResult(true);
        }
    }

    public sealed record ThrowingNotification : INotification;

    public sealed class ThrowingNotificationHandler : INotificationHandler<ThrowingNotification>
    {
        private static int _executionCount;
        public static int ExecutionCount => _executionCount;

        public static void Reset() => _executionCount = 0;

        public ValueTask Handle(ThrowingNotification notification, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _executionCount);
            throw new InvalidOperationException("boom");
        }
    }

    public sealed record DrainNotification : INotification;

    public sealed class BlockingNotificationHandler : INotificationHandler<DrainNotification>
    {
        public static TaskCompletionSource<bool> Started { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        public static TaskCompletionSource<bool> Release { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static void Reset()
        {
            Started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public async ValueTask Handle(DrainNotification notification, CancellationToken cancellationToken)
        {
            Started.TrySetResult(true);
            await Release.Task.WaitAsync(cancellationToken);
        }
    }

    public sealed class ObservingNotificationHandler : INotificationHandler<DrainNotification>
    {
        private static int _executionCount;
        public static int ExecutionCount => _executionCount;

        public static void Reset() => _executionCount = 0;

        public ValueTask Handle(DrainNotification notification, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _executionCount);
            return ValueTask.CompletedTask;
        }
    }
}
