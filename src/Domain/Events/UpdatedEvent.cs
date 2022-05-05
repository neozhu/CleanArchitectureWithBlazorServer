namespace CleanArchitecture.Blazor.Domain.Events;
public class UpdatedEvent<T> : DomainEvent
{
    public UpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
