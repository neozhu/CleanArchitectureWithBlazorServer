

//namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.EventHandlers;

//    public class InvoiceLineDeletedEventHandler : INotificationHandler<InvoiceLineDeletedEvent>
//    {
//        private readonly ILogger<InvoiceLineDeletedEventHandler> _logger;

//        public InvoiceLineDeletedEventHandler(
//            ILogger<InvoiceLineDeletedEventHandler> logger
//            )
//        {
//            _logger = logger;
//        }
//        public Task Handle(InvoiceLineDeletedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
//            return Task.CompletedTask;
//        }
//    }
