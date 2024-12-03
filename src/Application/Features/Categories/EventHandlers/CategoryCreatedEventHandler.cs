
namespace CleanArchitecture.Blazor.Application.Features.Categories.EventHandlers;

public class CategoryCreatedEventHandler : INotificationHandler<CategoryCreatedEvent>
{
        private readonly ILogger<CategoryCreatedEventHandler> _logger;

        public CategoryCreatedEventHandler(
            ILogger<CategoryCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(CategoryCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
            return Task.CompletedTask;
        }
}
