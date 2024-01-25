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
       .Where(e => e > RoleNamesEnum.HOSPITAL)
       .ToList();

    public static List<RoleNamesEnum> HospitalRoles = AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.HOSPITAL && e > RoleNamesEnum.DIAGNOSTICCENTER)
            .ToList();

    public static List<RoleNamesEnum> DiagnosticCenterRoles = AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.DIAGNOSTICCENTER && e > RoleNamesEnum.PHARMACY)
            .ToList();

    public static List<RoleNamesEnum> PharmacyRoles = AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.PHARMACY && e > RoleNamesEnum.PATIENT)
            .ToList();
    // public static List<string> DefaultTenantNames = DefaultTenantStructure.Select(t => t.Name).ToList();

    // public static List<TenantStructure> DefaultTenantStructure = GetDefaultTenantStructure();
    public static List<TenantStructure> GetDefaultTenantStructure()
    {
        //if (DefaultTenantStructure == null || !DefaultTenantStructure.Any())
        //{
        var defaultTenantStructure1 = new List<TenantStructure>() {
     new(TenantTypeEnum.Patient.ToString(),TenantTypeEnum.Patient,new List<ApplicationRole>(){ new ApplicationRole(RoleNamesEnum.PATIENT)}),
    new(TenantTypeEnum.Internal, ApplicationRole.CreateRolesForTenantType(InternalRoles, TenantTypeEnum.Internal)),

    //below are test data only
            new("Nanjappa HOSPITAL,Shimoga", TenantTypeEnum.HospitalAndStaff,ApplicationRole.CreateRolesForTenantType( HospitalRoles,TenantTypeEnum.HospitalAndStaff)),
              new("Sarji HOSPITAL,Shimogga", TenantTypeEnum.HospitalAndStaff,ApplicationRole.CreateRolesForTenantType(HospitalRoles,TenantTypeEnum.HospitalAndStaff)),
    new("Sarji PHARMACY,Shivamogga",TenantTypeEnum.PharmacyAndStaff,ApplicationRole.CreateRolesForTenantType(PharmacyRoles,TenantTypeEnum.PharmacyAndStaff)),
    new("Malnad DIAGNOSTIC Center,Bhadravathi", TenantTypeEnum.DiagnosticsAndStaff, ApplicationRole.CreateRolesForTenantType( DiagnosticCenterRoles,TenantTypeEnum.DiagnosticsAndStaff))

            };
        return defaultTenantStructure1;
        //}
        //return DefaultTenantStructure;
    }
    //todo GUEST role can be thought
    //todo need to add Permissions also for each roles

    //new("HospitalAndStaff","HOSPITAL And All Staff",new List<ApplicationRole>(){ new ApplicationRole(RoleName.HOSPITAL),new ApplicationRole(RoleName.HOSPITALADMIN),
    //new ApplicationRole(RoleName.HOSPITALADMIN),new ApplicationRole(RoleName.DOCTOR),new ApplicationRole(RoleName.DOCTORASSISTANT),
    //new ApplicationRole(RoleName.NURSE),new ApplicationRole(RoleName.VIEWERHOSPITAL)})
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
