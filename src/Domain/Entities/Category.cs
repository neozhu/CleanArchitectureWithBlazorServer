

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;
public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
}