using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Step : BaseAuditableEntity
{
    public string? Name { get; set; }
    public int? InvoiceId { get; set; }
    public bool IsCompleted { get; set; }
    public int StepOrder { get; set; }
    public ICollection<Comment> Comments { get; set; } = [];
}

