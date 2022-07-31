namespace CleanArchitecture.Blazor.$safeprojectname$.Events;
public class UpdatedEvent<T> : DomainEvent
{
    public UpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
