// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.



using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;

[Description("Audit Trails")]
public class AuditTrailDto
{
    [Display(Name = "Id")] public int Id { get; set; }
    [Display(Name = "User Id")] public string? UserId { get; set; }
    [Display(Name = "Audit Type")] public AuditType? AuditType { get; set; }
    [Display(Name = "Table Name")] public string? TableName { get; set; }
    [Display(Name = "Created DateTime")] public DateTime DateTime { get; set; }
    [Display(Name = "Changes")] public Dictionary<string, AuditChange>? Changes { get; set; }
    
    [Display(Name = "Affected Columns")] public List<string>? AffectedColumns { get; set; }
    [Display(Name = "Primary Key")] public Dictionary<string, string> PrimaryKey { get; set; }=new Dictionary<string, string>();
    [Display(Name = "Show Details")] public bool ShowDetails { get; set; }
    [Display(Name = "Owner")] public UserBriefDto? Owner { get; set; }}
