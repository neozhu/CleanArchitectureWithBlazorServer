

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.EventHandlers;

public class OfferLineCreatedEventHandler : INotificationHandler<OfferLineCreatedEvent>
{
        private readonly ILogger<OfferLineCreatedEventHandler> _logger;

        public OfferLineCreatedEventHandler(
            ILogger<OfferLineCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(OfferLineCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
            return Task.CompletedTask;
        }
}
