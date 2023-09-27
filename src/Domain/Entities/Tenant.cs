using CleanArchitecture.Blazor.Domain.Enums;

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
    public byte Type { get; set; } = (byte)Enums.TenantTypeEnum.Patient;
    public bool Active { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Today;
    public DateTime? ModifiedLastDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? CreatedByUser { get; set; }//TODO make it non nullable,as always someone exists
    public string? ModifiedLastByUser { get; set; }
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
    public new string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public byte Type { get; set; } = (byte)Enums.TenantTypeEnum.Patient;
    public bool Active { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Today;
    public DateTime? ModifiedLastDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? CreatedByUser { get; set; }//TODO make it non nullable,as always someone exists
    public string? ModifiedLastByUser { get; set; }
    public string? ApprovedByUser { get; set; }

}
