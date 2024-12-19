

//namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.EventHandlers;

//    public class InvoiceLineUpdatedEventHandler : INotificationHandler<InvoiceLineUpdatedEvent>
//    {
//        private readonly ILogger<InvoiceLineUpdatedEventHandler> _logger;

//        public InvoiceLineUpdatedEventHandler(
//            ILogger<InvoiceLineUpdatedEventHandler> logger
//            )
//        {
//            _logger = logger;
//        }
//        public Task Handle(InvoiceLineUpdatedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
//            return Task.CompletedTask;
//        }
//    }
