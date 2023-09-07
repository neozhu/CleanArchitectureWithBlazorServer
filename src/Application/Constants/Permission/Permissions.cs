// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Application.Constants.Permission;

public static class Permissions
{
    /*
    public const string PatientView = "Permissions.Patient.View";
    public const string PatientCreate = "Permissions.Patient.Create";
    public const string PatientEdit = "Permissions.Patient.Edit";
    public const string PatientDelete = "Permissions.Patient.Delete";
    public const string PatientSearch = "Permissions.Patient.Search";
    public const string PatientExport = "Permissions.Patient.Export";
    public const string PatientImport = "Permissions.Patient.Import";*/

    [DisplayName("AuditTrails")]
    [Description("AuditTrails Permissions")]
    public static class AuditTrails
    {
        public const string View = "Permissions.AuditTrails.View";
        public const string Search = "Permissions.AuditTrails.Search";
        public const string Export = "Permissions.AuditTrails.Export";
    }
    [DisplayName("Logs")]
    [Description("Logs Permissions")]
    public static class Logs
    {
        public const string View = "Permissions.Logs.View";
        public const string Search = "Permissions.Logs.Search";
        public const string Export = "Permissions.Logs.Export";
        public const string Purge = "Permissions.Logs.Purge";
    }


    [DisplayName("Products")]
    [Description("Products Permissions")]
    public static class Products
    {
        public const string View = "Permissions.Products.View";
        public const string Create = "Permissions.Products.Create";
        public const string Edit = "Permissions.Products.Edit";
        public const string Delete = "Permissions.Products.Delete";
        public const string Search = "Permissions.Products.Search";
        public const string Export = "Permissions.Products.Export";
        public const string Import = "Permissions.Products.Import";
    }
    [DisplayName("Customers")]
    [Description("Customers Permissions")]
    public static class Customers
    {
        public const string View = "Permissions.Customers.View";
        public const string Create = "Permissions.Customers.Create";
        public const string Edit = "Permissions.Customers.Edit";
        public const string Delete = "Permissions.Customers.Delete";
        public const string Search = "Permissions.Customers.Search";
        public const string Export = "Permissions.Customers.Export";
        public const string Import = "Permissions.Customers.Import";
    }

    [DisplayName("Categories")]
    [Description("Categories Permissions")]
    public static class Categories
    {
        public const string View = "Permissions.Categories.View";
        public const string Create = "Permissions.Categories.Create";
        public const string Edit = "Permissions.Categories.Edit";
        public const string Delete = "Permissions.Categories.Delete";
        public const string Search = "Permissions.Categories.Search";
        public const string Export = "Permissions.Categories.Export";
        public const string Import = "Permissions.Categories.Import";
    }

    [DisplayName("Documents")]
    [Description("Documents Permissions")]
    public static class Documents
    {
        public const string View = "Permissions.Documents.View";
        public const string Create = "Permissions.Documents.Create";
        public const string Edit = "Permissions.Documents.Edit";
        public const string Delete = "Permissions.Documents.Delete";
        public const string Search = "Permissions.Documents.Search";
        public const string Export = "Permissions.Documents.Export";
        public const string Import = "Permissions.Documents.Import";
        public const string Download = "Permissions.Documents.Download";
    }
    [DisplayName("Dictionaries")]
    [Description("Dictionaries Permissions")]
    public static class Dictionaries
    {
        public const string View = "Permissions.Dictionaries.View";
        public const string Create = "Permissions.Dictionaries.Create";
        public const string Edit = "Permissions.Dictionaries.Edit";
        public const string Delete = "Permissions.Dictionaries.Delete";
        public const string Search = "Permissions.Dictionaries.Search";
        public const string Export = "Permissions.Dictionaries.Export";
        public const string Import = "Permissions.Dictionaries.Import";
    }

    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Search = "Permissions.Users.Search";
        public const string Import = "Permissions.Users.Import";
        public const string Export = "Permissions.Dictionaries.Export";
        public const string ManageRoles = "Permissions.Users.ManageRoles";
        public const string RestPassword = "Permissions.Users.RestPassword";
        public const string Active = "Permissions.Users.Active";
        public const string ManagePermissions = "Permissions.Users.Permissions";
    }

    [DisplayName("Roles")]
    [Description("Roles Permissions")]
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
        public const string Search = "Permissions.Roles.Search";
        public const string Export = "Permissions.Roles.Export";
        public const string Import = "Permissions.Roles.Import";
        public const string ManagePermissions = "Permissions.Roles.Permissions";
        public const string ManageNavigation = "Permissions.Roles.Navigation";
    }
    [DisplayName("Multi-Tenant")]
    [Description("Multi-Tenant Permissions")]
    public static class Tenants
    {
        public const string View = "Permissions.Tenants.View";
        public const string Create = "Permissions.Tenants.Create";
        public const string Edit = "Permissions.Tenants.Edit";
        public const string Delete = "Permissions.Tenants.Delete";
        public const string Search = "Permissions.Tenants.Search";
    }
    [DisplayName("Role Claims")]
    [Description("Role Claims Permissions")]
    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Edit = "Permissions.RoleClaims.Edit";
        public const string Delete = "Permissions.RoleClaims.Delete";
        public const string Search = "Permissions.RoleClaims.Search";
    }



    [DisplayName("Dashboards")]
    [Description("Dashboards Permissions")]
    public static class Dashboards
    {
        public const string View = "Permissions.Dashboards.View";
    }

    [DisplayName("Hangfire")]
    [Description("Hangfire Permissions")]
    public static class Hangfire
    {
        public const string View = "Permissions.Hangfire.View";
        public const string Jobs = "Permissions.Hangfire.Jobs";
    }


    /// <summary>
    /// Returns a list of Permissions.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add((string)propertyValue);
        }
        return permissions;
    }

    public class RootAdmin
    {
        public class ElevateAdminGroup
        {
            static RoleAndPermission patient = new(RoleNamesEnum.Patient);
            static RoleAndPermission pharmacist = new(RoleNamesEnum.Pharmacist);
            static RoleAndPermission pharmacy = new(RoleNamesEnum.Pharmacy);
            static RoleAndPermission diagnostic = new(RoleNamesEnum.Diagnostic);
            static RoleAndPermission diagnosticCenter = new(RoleNamesEnum.DiagnosticCenter);
            static RoleAndPermission viewerHospital = new(RoleNamesEnum.ViewerHospital);
            static RoleAndPermission nurse = new(RoleNamesEnum.Nurse);
            static RoleAndPermission doctorAssistant = new(RoleNamesEnum.DoctorAssistant);
            static RoleAndPermission doctor = new(RoleNamesEnum.Doctor);
            static RoleAndPermission doctorHOD = new(RoleNamesEnum.DoctorHOD);
            static RoleAndPermission hospitalAdmin = new(RoleNamesEnum.HospitalAdmin);
            static RoleAndPermission hospital = new(RoleNamesEnum.Hospital);
            static RoleAndPermission elevateAdminViewer = new(RoleNamesEnum.ElevateAdminViewer);
            static RoleAndPermission elevateAdminGroup = new(RoleNamesEnum.ElevateAdminGroup);
            static RoleAndPermission rootAdmin = new(RoleNamesEnum.RootAdmin);

            public class ElevateAdminViewersPermissions
            {
                public class HospitalPermissions
                {
                    public class HospitalAdminPermissions
                    {
                        public class DoctorHODPermissions
                        {
                            public class DoctorPermissions
                            {
                                public class DoctorAssistantPermissions
                                {
                                    public class NursePermissions
                                    {
                                        public class ViewerHospitalPermissions
                                        {
                                            public class DiagnosticCenterPermissions
                                            {
                                                public class DiagnosticsPermissions
                                                {
                                                    public class PharmacyPermissions
                                                    {
                                                        public string pharmacistUnAssign = pharmacist.ToString(PermissionsEnum.UnAssign);

                                                        public string pharmacyAssign = pharmacy.ToString(PermissionsEnum.Assign);
                                                        public string pharmacyCreate = pharmacy.ToString(PermissionsEnum.Create);
                                                        public string pharmacyEdit = pharmacy.ToString(PermissionsEnum.Update);
                                                        public class PharmacistsPermissions //any more equivalent roles like attendants,operators all can be added here
                                                        {
                                                            //here almost all reading 
                                                            public string PatientRead = patient.ToString(PermissionsEnum.Read);
                                                            public string PharmacistsRead = pharmacist.ToString(PermissionsEnum.Read);
                                                            public string PharmacyRead = pharmacy.ToString(PermissionsEnum.Read);
                                                            public string DiagnosticsRead = diagnostic.ToString(PermissionsEnum.Read);
                                                            public string DiagnosticCenterRead = diagnosticCenter.ToString(PermissionsEnum.Read);
                                                            public string ViewerHospitalRead = viewerHospital.ToString(PermissionsEnum.Read);
                                                            public string NurseRead = nurse.ToString(PermissionsEnum.Read);
                                                            public string DoctorAssistantRead = doctorAssistant.ToString(PermissionsEnum.Read);
                                                            public string DoctorRead = doctor.ToString(PermissionsEnum.Read);
                                                            public string DoctorHODRead = doctorHOD.ToString(PermissionsEnum.Read);
                                                            public string HospitalAdminRead = hospitalAdmin.ToString(PermissionsEnum.Read);
                                                            public string HospitalRead = hospital.ToString(PermissionsEnum.Read);

                                                            public string PatientCreate = patient.ToString(PermissionsEnum.Create);
                                                            public string PatientUpdate = patient.ToString(PermissionsEnum.Update);

                                                            public string PharmacistsAssign = pharmacist.ToString(PermissionsEnum.Assign);
                                                            public string PharmacistsCreate = pharmacist.ToString(PermissionsEnum.Create);
                                                            public string PharmacistsEdit = pharmacist.ToString(PermissionsEnum.Update);
                                                            public class PatientPermissions
                                                            {
                                                                public string PatientReadRestricted = patient.ToString(PermissionsEnum.ReadRestricted);
                                                                public string PatientCreateRestricted = patient.ToString(PermissionsEnum.CreateRestricted);
                                                                public string PatientEditRestricted = patient.ToString(PermissionsEnum.UpdateRestricted);//allows upto base limit & only self created 
                                                                public string PatientDeleteRestricted = patient.ToString(PermissionsEnum.DeleteRestricted);//allows upto base limit & only self created 
                                                                                                                                                           //bills,prescriptions,transactions +can see all doctors,hospitals,diagnostic,pharmacy
                                                                public static class AllRegisteredUserPermissions
                                                                {
                                                                    //can see all doctors,hospitals,diagnostic,pharmacy
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public class RoleAndPermission
    {
        private RoleNamesEnum Role { get; set; }
        private PermissionsEnum? Permission { get; set; } = null;
        public RoleAndPermission(RoleNamesEnum _role)
        {
            Role = _role;
        }
        public RoleAndPermission(RoleNamesEnum _role, PermissionsEnum _permission)
        {
            Role = _role;
            Permission = _permission;
        }
        public string ToString(PermissionsEnum permission)
        {
            return permission == PermissionsEnum.Delete ? string.Empty : Role.ToString() + permission.ToString();
        }
        public override string ToString()
        {
            return (Permission == null || Permission == PermissionsEnum.Delete) ? string.Empty : Role.ToString() + Permission.ToString();
        }
    }


}
