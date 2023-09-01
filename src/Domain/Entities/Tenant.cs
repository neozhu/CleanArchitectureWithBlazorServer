namespace CleanArchitecture.Blazor.Domain.Entities;
public class Tenant : IEntity<string>
{
    public Tenant()
    {

    }
    public Tenant(string name, string description)
    {
        Name = name; Description = description;
    }
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public byte Type { get; set; } = (byte)Enums.TenantType.Patient;
}
