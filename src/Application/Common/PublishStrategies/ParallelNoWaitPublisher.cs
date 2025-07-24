namespace CleanArchitecture.Blazor.Application.Common.PublishStrategies;

public class ParallelNoWaitPublisher : INotificationPublisher
{
    public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification,
        CancellationToken cancellationToken)
    {
        var handlers = handlerExecutors.ToList();
        
        if (!handlers.Any())
            return Task.CompletedTask;

        foreach (var handler in handlers)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // 静默处理异常，保持 NoWait 语义
                }
            }, cancellationToken);
        }

        return Task.CompletedTask;
    }
}