using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Comment : BaseAuditableEntity
{
    public int StepId { get; set; }
    public string? Content { get; set; }
}

