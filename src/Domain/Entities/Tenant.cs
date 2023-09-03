using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;
public class Tenant : IEntity<string>
{
    public Tenant()
    {

    }

    public Tenant(string name, string description, TenantTypeEnum type) : this(name, description, (byte)type)
    { }
    public Tenant(string name, string description, byte type)
    {
        Name = name; Description = description; Type = (byte)type;
    }
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public byte Type { get; set; } = (byte)Enums.TenantTypeEnum.Patient;
}
