
using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class SubProduct : BaseAuditableEntity
{
    public int ProductId { get; set; } // Foreign Key
    public string? Unit { get; set; }
    public string? Color { get; set; }

    public Product Product { get; set; } = null!;
}