namespace CleanArchitecture.Blazor.Domain.Entities;
public class Tenant : BaseAuditableEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
}
