// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Documents.DTOs;

[Description("Documents")]
public class DocumentDto
{
    [Display(Name = "Id")] public int Id { get; set; }
    [Display(Name = "Title")] public string? Title { get; set; }
    [Display(Name = "Description")] public string? Description { get; set; }
    [Display(Name = "Is Public")] public bool IsPublic { get; set; }
    [Display(Name = "URL")] public string? URL { get; set; }
    [Display(Name = "Document Type")] public DocumentType DocumentType { get; set; } = DocumentType.Document;
    [Display(Name = "Tenant Id")] public string? TenantId { get; set; }
    [Display(Name = "Tenant Name")] public string? TenantName { get; set; }
    [Display(Name = "Status")] public JobStatus Status { get; set; } = JobStatus.NotStart;
    [Display(Name = "Content")] public string? Content { get; set; }
    [Display(Name = "Created By")] public UserBriefDto? CreatedBy { get; set; }
    [Display(Name = "Created At")] public DateTime? CreatedAt { get; set; }}
