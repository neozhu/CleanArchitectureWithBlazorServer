using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using Mediator;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.MediatorCompatibility;

public class ParallelNoWaitPublisherTests
{
    [Test]
    public async Task Publish_returns_before_handler_completion()
    {
        var handlerStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseHandler = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var publisher = new ParallelNoWaitPublisher();
        var notification = new TestNotification();
        var handler = new TestHandler(handlerStarted, releaseHandler);
        var handlers = new NotificationHandlers<TestNotification>(new[] { handler }, true);

        ValueTask publishTask = publisher.Publish(handlers, notification, CancellationToken.None);

        Assert.That(publishTask.IsCompleted, Is.True);

        await handlerStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.That(handlerStarted.Task.IsCompleted, Is.True);
        Assert.That(releaseHandler.Task.IsCompleted, Is.False);

        releaseHandler.SetResult(true);
        await publishTask;
    }

    private sealed class TestHandler : INotificationHandler<TestNotification>
    {
        private readonly TaskCompletionSource<bool> _handlerStarted;
        private readonly TaskCompletionSource<bool> _releaseHandler;

        public TestHandler(TaskCompletionSource<bool> handlerStarted, TaskCompletionSource<bool> releaseHandler)
        {
            _handlerStarted = handlerStarted;
            _releaseHandler = releaseHandler;
        }

        public async ValueTask Handle(TestNotification notification, CancellationToken cancellationToken)
        {
            _handlerStarted.TrySetResult(true);
            await _releaseHandler.Task.WaitAsync(cancellationToken);
        }
    }

    private sealed record TestNotification : INotification;
}
