

//namespace CleanArchitecture.Blazor.Application.Invoices.EventHandlers;

//    public class InvoiceDeletedEventHandler : INotificationHandler<InvoiceDeletedEvent>
//    {
//        private readonly ILogger<InvoiceDeletedEventHandler> _logger;

//        public InvoiceDeletedEventHandler(
//            ILogger<InvoiceDeletedEventHandler> logger
//            )
//        {
//            _logger = logger;
//        }
//        public Task Handle(InvoiceDeletedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
//            return Task.CompletedTask;
//        }
//    }
