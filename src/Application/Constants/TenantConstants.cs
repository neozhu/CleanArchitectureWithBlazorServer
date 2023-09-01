using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Constants.Role;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Constants;
public class TenantBase
{
    public static List<string> DefaultTenantNames = DefaultTenantStructure.Select(t => t.Name).ToList();

    public static  List<TenantStructure> DefaultTenantStructure = new() {
    new("Patient", "Patient",new List<ApplicationRole>(){ new ApplicationRole(RoleName.Patient)}),
    new("HospitalAndStaff","Hospital And All Staff",ApplicationRole.CreateRolesForTenantType( new List<string>(){RoleName.Hospital,RoleName.HospitalAdmin,
            RoleName.HospitalAdmin,RoleName.Doctor,RoleName.DoctorAssistant,RoleName.Nurse,RoleName.ViewerHospital },TenantType.Hospital)),
    new("PharmacyAndStaff","Pharmacy And All Staff",ApplicationRole.CreateRolesForTenantType( new List<string>(){RoleName.Pharmacy,RoleName.Pharmacists},TenantType.Pharmacy)),
    new("DiagnosticsAndStaff","Diagnostic Center And All Staff",ApplicationRole.CreateRolesForTenantType( new List<string>(){RoleName.DiagnosticCenter,RoleName.Diagnostics},TenantType.DiagnosticCenter)),
    new("Internal","Internal",ApplicationRole.CreateRolesForTenantType( new List<string>(){RoleName.RootAdmin, RoleName.ElevateAdminGroup,RoleName.ElevateAdminViewers},TenantType.Internal)),
    };//todo Guest role can be thought
    //todo need to add Permissions also for each roles

    //new("HospitalAndStaff","Hospital And All Staff",new List<ApplicationRole>(){ new ApplicationRole(RoleName.Hospital),new ApplicationRole(RoleName.HospitalAdmin),
    //new ApplicationRole(RoleName.HospitalAdmin),new ApplicationRole(RoleName.Doctor),new ApplicationRole(RoleName.DoctorAssistant),
    //new ApplicationRole(RoleName.Nurse),new ApplicationRole(RoleName.ViewerHospital)})
}

public class TenantStructure
{
    public string? Id { get; set; }
    public byte Level { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ApplicationRole> Roles { get; set; }
    //public TenantStructure()
    //{

    //}
    public TenantStructure(string name, string description, List<ApplicationRole> roles)
    {
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
}
