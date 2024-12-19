
//namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.EventHandlers;

//public class InvoiceLineCreatedEventHandler : INotificationHandler<InvoiceLineCreatedEvent>
//{
//        private readonly ILogger<InvoiceLineCreatedEventHandler> _logger;

//        public InvoiceLineCreatedEventHandler(
//            ILogger<InvoiceLineCreatedEventHandler> logger
//            )
//        {
//            _logger = logger;
//        }
//        public Task Handle(InvoiceLineCreatedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
//            return Task.CompletedTask;
//        }
//}
