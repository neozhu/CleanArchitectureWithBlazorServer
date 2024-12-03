
namespace CleanArchitecture.Blazor.Application.Features.Categories.EventHandlers;

    public class CategoryUpdatedEventHandler : INotificationHandler<CategoryUpdatedEvent>
    {
        private readonly ILogger<CategoryUpdatedEventHandler> _logger;

        public CategoryUpdatedEventHandler(
            ILogger<CategoryUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(CategoryUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
            return Task.CompletedTask;
        }
    }
