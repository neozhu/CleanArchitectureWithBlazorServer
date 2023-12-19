// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Common.Entities;

public abstract class BaseAuditableEntity : BaseEntity
{
    public virtual DateTime? Created { get; set; }

    public virtual string? CreatedBy { get; set; }

    public virtual DateTime? LastModified { get; set; }

    public virtual string? LastModifiedBy { get; set; }
}