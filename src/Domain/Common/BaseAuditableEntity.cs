// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Identity;

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
    public virtual DateTime? Created { get; set; }

    public virtual string? CreatedBy { get; set; }

    public virtual DateTime? LastModified { get; set; }

    public virtual string? LastModifiedBy { get; set; }
}

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }
    string? DeletedBy { get; set; }
}
public abstract class BaseAuditableSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }

}


public abstract class OwnerPropertyEntity: BaseAuditableEntity
{
    [ForeignKey("CreatedBy")]
    public virtual ApplicationUser? Owner { get; set; }
    [ForeignKey("LastModifiedBy")]
    public virtual ApplicationUser? Editor { get; set; }
}
