

//namespace CleanArchitecture.Blazor.Application.Features.OfferLines.EventHandlers;

//    public class OfferLineUpdatedEventHandler : INotificationHandler<OfferLineUpdatedEvent>
//    {
//        private readonly ILogger<OfferLineUpdatedEventHandler> _logger;

//        public OfferLineUpdatedEventHandler(
//            ILogger<OfferLineUpdatedEventHandler> logger
//            )
//        {
//            _logger = logger;
//        }
//        public Task Handle(OfferLineUpdatedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
//            return Task.CompletedTask;
//        }
//    }
