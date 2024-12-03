
namespace CleanArchitecture.Blazor.Application.Features.Categories.EventHandlers;

    public class CategoryDeletedEventHandler : INotificationHandler<CategoryDeletedEvent>
    {
        private readonly ILogger<CategoryDeletedEventHandler> _logger;

        public CategoryDeletedEventHandler(
            ILogger<CategoryDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(CategoryDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
            return Task.CompletedTask;
        }
    }
