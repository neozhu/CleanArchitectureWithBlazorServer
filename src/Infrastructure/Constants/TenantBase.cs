using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Identity;
using DocumentFormat.OpenXml.Wordprocessing;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Common.Enums;

namespace CleanArchitecture.Blazor.Application.Constants;
public class TenantBase
{
    internal static List<RoleNamesEnum> AllRoleNameEnums = Enum.GetValues(typeof(RoleNamesEnum))
        .Cast<RoleNamesEnum>()
        .ToList();
    public static List<RoleNamesEnum> InternalRoles = AllRoleNameEnums
       .Where(e => e > RoleNamesEnum.Hospital)
       .ToList();

    public static List<RoleNamesEnum> HospitalRoles = AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.Hospital && e > RoleNamesEnum.DiagnosticCenter)
            .ToList();

    public static List<RoleNamesEnum> DiagnosticCenterRoles = AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.DiagnosticCenter && e > RoleNamesEnum.Pharmacy)
            .ToList();

    public static List<RoleNamesEnum> PharmacyRoles = AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.Pharmacy && e > RoleNamesEnum.Patient)
            .ToList();
    // public static List<string> DefaultTenantNames = DefaultTenantStructure.Select(t => t.Name).ToList();

    // public static List<TenantStructure> DefaultTenantStructure = GetDefaultTenantStructure();
    public static List<TenantStructure> GetDefaultTenantStructure()
    {
        //if (DefaultTenantStructure == null || !DefaultTenantStructure.Any())
        //{
        var defaultTenantStructure1 = new List<TenantStructure>() {
     new(TenantTypeEnum.Patient.ToString(),TenantTypeEnum.Patient,new List<ApplicationRole>(){ new ApplicationRole(RoleNamesEnum.Patient)}),
    new(TenantTypeEnum.Internal, ApplicationRole.CreateRolesForTenantType(InternalRoles, TenantTypeEnum.Internal)),

    //below are test data only
            new("Nanjappa Hospital,Shimoga", TenantTypeEnum.HospitalAndStaff,ApplicationRole.CreateRolesForTenantType( HospitalRoles,TenantTypeEnum.HospitalAndStaff)),
              new("Sarji Hospital,Shimogga", TenantTypeEnum.HospitalAndStaff,ApplicationRole.CreateRolesForTenantType(HospitalRoles,TenantTypeEnum.HospitalAndStaff)),
    new("Sarji Pharmacy,Shivamogga",TenantTypeEnum.PharmacyAndStaff,ApplicationRole.CreateRolesForTenantType(PharmacyRoles,TenantTypeEnum.PharmacyAndStaff)),
    new("Malnad Diagnostic Center,Bhadravathi", TenantTypeEnum.DiagnosticsAndStaff, ApplicationRole.CreateRolesForTenantType( DiagnosticCenterRoles,TenantTypeEnum.DiagnosticsAndStaff))

            };
        return defaultTenantStructure1;
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
    public string? Id { get; set; }
    public byte Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ApplicationRole> Roles { get; set; }
    //public TenantStructure()
    //{

    //}
    public TenantStructure(TenantTypeEnum type, List<ApplicationRole> roles)
        : this(type.ToString(), type, roles, type.GetEnumDescription())
    {

    }
    public TenantStructure(string name, TenantTypeEnum type, List<ApplicationRole> roles, string? description = null)
    {
        Type = (byte)type;
        Name = name;
        Description = string.IsNullOrEmpty(description) ? name : description;
        Roles = roles;
    }
    public static Tenant Tenant(TenantStructure tenant)
    {
        return new Tenant(tenant.Name, tenant.Description, tenant.Type);
    }
}
