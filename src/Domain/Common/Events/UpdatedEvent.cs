using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Common.Events;

public class UpdatedEvent<T> : DomainEvent where T : IEntity
{
    public UpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}