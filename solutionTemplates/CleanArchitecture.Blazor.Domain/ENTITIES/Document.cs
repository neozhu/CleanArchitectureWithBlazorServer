// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.$safeprojectname$.Entities;

public class Document : AuditableEntity, IMayHaveTenant
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public string? URL { get; set; }
    public DocumentType DocumentType { get; set; } = default!;
    public string? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
