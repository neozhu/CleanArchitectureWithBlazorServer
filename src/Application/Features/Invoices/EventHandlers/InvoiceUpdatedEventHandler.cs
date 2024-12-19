
//namespace CleanArchitecture.Blazor.Application.Invoices.EventHandlers;

//    public class InvoiceUpdatedEventHandler : INotificationHandler<InvoiceUpdatedEvent>
//    {
//        private readonly ILogger<InvoiceUpdatedEventHandler> _logger;

//        public InvoiceUpdatedEventHandler(
//            ILogger<InvoiceUpdatedEventHandler> logger
//            )
//        {
//            _logger = logger;
//        }
//        public Task Handle(InvoiceUpdatedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
//            return Task.CompletedTask;
//        }
//    }
