
//namespace CleanArchitecture.Blazor.Application.Invoices.EventHandlers;

//public class InvoiceCreatedEventHandler : INotificationHandler<InvoiceCreatedEvent>
//{
//        private readonly ILogger<InvoiceCreatedEventHandler> _logger;

//        public InvoiceCreatedEventHandler(
//            ILogger<InvoiceCreatedEventHandler> logger
//            )
//        {
//            _logger = logger;
//        }
//        public Task Handle(InvoiceCreatedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
//            return Task.CompletedTask;
//        }
//}
