using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Common.Entities;
namespace CleanArchitecture.Blazor.Domain.Entities;
public class TenantPending : BaseAuditableEntity, IEntity<string>
{
    public TenantPending()
    { }

    public TenantPending(string name, string description, TenantTypeEnum type) : this(name, description, (byte)type)
    { }
    public TenantPending(string name, string description, byte type)
    {
        Name = name; Description = description; Type = type;
    }
    public new string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public byte Type { get; set; } = (byte)Enums.TenantTypeEnum.HospitalAndStaff;
    public bool Active { get; set; } = true;

    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedByUser { get; set; }
}
public class Tenant : BaseAuditableEntity, IEntity<string>
{
    public Tenant()
    {

    }

    public Tenant(string name, string description, TenantTypeEnum type) : this(name, description, (byte)type)
    { }
    public Tenant(string name, string description, byte type)
    {
        Name = name; Description = description; Type =type;
    }
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public byte Type { get; set; } = (byte)Enums.TenantTypeEnum.HospitalAndStaff;
    public bool Active { get; set; } = true;

    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedByUser { get; set; }

}
