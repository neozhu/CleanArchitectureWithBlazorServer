namespace CleanArchitecture.Blazor.Application.Common.PublishStrategies;

public class ParallelNoWaitPublisher : INotificationPublisher
{
    private readonly ILogger<ParallelNoWaitPublisher> _logger;

    public ParallelNoWaitPublisher(ILogger<ParallelNoWaitPublisher> logger)
    {
        _logger = logger;
    }

    public ValueTask Publish<TNotification>(NotificationHandlers<TNotification> handlers, TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var handlerList = handlers.ToList();
        
        if (!handlerList.Any())
            return ValueTask.CompletedTask;

        foreach (var handler in handlerList)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await handler.Handle(notification, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Log exception while maintaining NoWait semantics
                    _logger.LogError(ex, "Failed to execute notification handler for {NotificationType}", notification.GetType().Name);
                }
            }, cancellationToken);
        }

        return ValueTask.CompletedTask;
    }
}
