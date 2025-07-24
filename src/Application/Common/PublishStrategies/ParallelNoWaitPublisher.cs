using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Common.PublishStrategies;

public class ParallelNoWaitPublisher : INotificationPublisher
{
    private readonly ILogger<ParallelNoWaitPublisher> _logger;

    public ParallelNoWaitPublisher(ILogger<ParallelNoWaitPublisher> logger)
    {
        _logger = logger;
    }

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
                catch (Exception ex)
                {
                    // Log exception while maintaining NoWait semantics
                    _logger.LogError(ex, "Failed to execute notification handler for {NotificationType}", notification.GetType().Name);
                }
            }, cancellationToken);
        }

        return Task.CompletedTask;
    }
}