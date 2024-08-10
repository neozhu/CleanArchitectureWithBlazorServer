using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;

public enum AuditTrailListView
{
    [Description("الكل")] [Display(Name="الكل")] All,
    [Description("My Change Histories")] My,
    [Description("Created Toady")] CreatedToday,
    [Description("View of the last 30 days")] Last30days
}

public class AuditTrailAdvancedFilter : PaginationFilter
{
    public AuditType? AuditType { get; set; }
    public AuditTrailListView ListView { get; set; } = AuditTrailListView.All;
    public UserProfile? CurrentUser { get; set; }
}