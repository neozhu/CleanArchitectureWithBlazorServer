// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Domain.Common.Entities;

public abstract class OwnerPropertyEntity : BaseAuditableEntity
{
    [ForeignKey("CreatedBy")] public virtual ApplicationUser? Owner { get; set; }

    [ForeignKey("LastModifiedBy")] public virtual ApplicationUser? LastModifier { get; set; }
}