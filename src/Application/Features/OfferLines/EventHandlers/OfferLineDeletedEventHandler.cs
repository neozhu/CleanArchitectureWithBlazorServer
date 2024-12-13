
namespace CleanArchitecture.Blazor.Application.Features.OfferLines.EventHandlers;

    public class OfferLineDeletedEventHandler : INotificationHandler<OfferLineDeletedEvent>
    {
        private readonly ILogger<OfferLineDeletedEventHandler> _logger;

        public OfferLineDeletedEventHandler(
            ILogger<OfferLineDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(OfferLineDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
            return Task.CompletedTask;
        }
    }
