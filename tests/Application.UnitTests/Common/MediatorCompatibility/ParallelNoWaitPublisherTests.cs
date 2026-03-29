using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
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

        Mediator.NotificationHandlerExecutor handlerExecutor = new(
            async (_, _) =>
            {
                handlerStarted.TrySetResult(true);
                await releaseHandler.Task;
            });

        Task publishTask = publisher.Publish(
            new[] { handlerExecutor },
            notification,
            CancellationToken.None);

        Assert.That(publishTask.IsCompleted, Is.True);

        await handlerStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.That(handlerStarted.Task.IsCompleted, Is.True);
        Assert.That(releaseHandler.Task.IsCompleted, Is.False);

        releaseHandler.SetResult(true);
        await publishTask;
    }

    private sealed record TestNotification : Mediator.INotification;
}
