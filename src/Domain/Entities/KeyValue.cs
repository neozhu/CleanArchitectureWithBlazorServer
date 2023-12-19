// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class KeyValue : BaseAuditableEntity, IAuditTrial
{
    public Picklist Name { get; set; } = Picklist.Brand;
    public string? Value { get; set; }
    public string? Text { get; set; }
    public string? Description { get; set; }
}

public enum Picklist
{
    [Description("Status")] Status,
    [Description("Unit")] Unit,
    [Description("Brand")] Brand
}