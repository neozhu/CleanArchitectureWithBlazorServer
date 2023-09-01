using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Constants.Role;
using CleanArchitecture.Blazor.Domain.Identity;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CleanArchitecture.Blazor.Application.Constants;
public class TenantBase
{
   // public static List<string> DefaultTenantNames = DefaultTenantStructure.Select(t => t.Name).ToList();

   // public static List<TenantStructure> DefaultTenantStructure = GetDefaultTenantStructure();
    public static List<TenantStructure> GetDefaultTenantStructure()
    {
        //if (DefaultTenantStructure == null || !DefaultTenantStructure.Any())
        //{
            var DefaultTenantStructure1 = new List<TenantStructure>() {
     new(TenantType.Patient,new List<ApplicationRole>(){ new ApplicationRole(RoleName.Patient)}),
    new(TenantType.HospitalAndStaff,ApplicationRole.CreateRolesForTenantType( new List<string>(){RoleName.Hospital,RoleName.HospitalAdmin,
            RoleName.Doctor,RoleName.DoctorAssistant,RoleName.Nurse,RoleName.ViewerHospital },TenantType.HospitalAndStaff)),
    new(TenantType.PharmacyAndStaff,ApplicationRole.CreateRolesForTenantType( new List<string>(){RoleName.Pharmacy,RoleName.Pharmacists},TenantType.PharmacyAndStaff)),
    new(TenantType.DiagnosticsAndStaff, ApplicationRole.CreateRolesForTenantType( new List<string>(){RoleName.DiagnosticCenter,RoleName.Diagnostics},TenantType.DiagnosticsAndStaff)),
    new(TenantType.Internal, ApplicationRole.CreateRolesForTenantType(new List < string >() { RoleName.RootAdmin, RoleName.ElevateAdminGroup, RoleName.ElevateAdminViewers }, TenantType.Internal))
            };
           return DefaultTenantStructure1;
        //}
        //return DefaultTenantStructure;
    }
    //todo Guest role can be thought
    //todo need to add Permissions also for each roles

    //new("HospitalAndStaff","Hospital And All Staff",new List<ApplicationRole>(){ new ApplicationRole(RoleName.Hospital),new ApplicationRole(RoleName.HospitalAdmin),
    //new ApplicationRole(RoleName.HospitalAdmin),new ApplicationRole(RoleName.Doctor),new ApplicationRole(RoleName.DoctorAssistant),
    //new ApplicationRole(RoleName.Nurse),new ApplicationRole(RoleName.ViewerHospital)})
}

public class TenantStructure
{
    public byte Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ApplicationRole> Roles { get; set; }
    //public TenantStructure()
    //{

    //}
    public TenantStructure(TenantType type, List<ApplicationRole> roles)
        : this((byte)type, type.ToString(), type.GetEnumDescription(), roles)
    {

    }
    public TenantStructure(TenantType type, string name, string description, List<ApplicationRole> roles) : this((byte)type, name, description, roles)
    { }
    public TenantStructure(byte type, string name, string description, List<ApplicationRole> roles)
    {
        Type = type;
        Name = name;
        Description = description;
        Roles = roles;
    }
    public TenantStructure(string name, List<ApplicationRole> roles)
    {
        Name = name;
        Description = name;
        Roles = roles;
    }
    public Tenant Tenant(TenantStructure tenant)
    {
        return new Tenant(tenant.Name, tenant.Description, tenant.Type);
    }
}
