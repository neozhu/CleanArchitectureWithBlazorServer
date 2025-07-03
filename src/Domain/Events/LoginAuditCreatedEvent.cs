using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Domain.Events;

    public class LoginAuditCreatedEvent : DomainEvent
    {
        public LoginAuditCreatedEvent(LoginAudit item)
        {
            Item = item;
        }

        public LoginAudit Item { get; }
    }

