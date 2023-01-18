// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Blazor.Domain.Common;

public interface IEntity
{

}

public abstract class BaseEntity: IEntity
{
    private readonly List<DomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }
    string? DeletedBy { get; set; }
}
public abstract class AuditableSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }

}
