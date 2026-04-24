// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

[Description("Roles")]
public class ApplicationRoleDto
{
    [Display(Name ="Id")] 
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;
    [Display(Name = "Normalized Name")] 
    public string? NormalizedName { get; set; }
    [Display(Name =  "Description")]
    public string? Description { get; set; }}
