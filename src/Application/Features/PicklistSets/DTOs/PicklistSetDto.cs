// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

[Description("Picklist")]
public class PicklistSetDto
{
    [Display(Name ="Id")] public int Id { get; set; }
    [Display(Name = "Name")] public Picklist Name { get; set; }
    [Display(Name = "Value")] public string? Value { get; set; }
    [Display(Name = "Text")] public string? Text { get; set; }
    [Display(Name = "Description")] public string? Description { get; set; }
    public TrackingState TrackingState { get; set; } = TrackingState.Unchanged;}
