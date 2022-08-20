// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace CleanArchitecture.Blazor.Domain.Events;
public class CreatedEvent<T> : DomainEvent where T : BaseEntity
{
    public CreatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
