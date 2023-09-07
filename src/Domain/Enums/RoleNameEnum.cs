using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Domain.Enums;
public enum RoleNamesEnum : byte
{
    RootAdmin = 50,
    ElevateAdminGroup = 45,
    ElevateAdminViewers = 41,

    Hospital = 30,
    HospitalAdmin = 29,
    DoctorHOD = 28,
    Doctor = 27,
    DoctorAssistant = 26,
    Nurse = 25,
    ViewerHospital = 21,

    DiagnosticCenter = 20,
    Diagnostics = 16,

    Pharmacy = 15,
    Pharmacists = 11,

    Patient = 4,

    [Description(nameof(Guest))]
    Guest = 1,
    Rejected = 0,
    // public const string Guest = nameof(Guest);//vmadhu2023 =>requesting for Hospital registration , vmadhu203=>patient   ,   vmadhu2022 =>req for doctor 
    DefaultRole1 = Patient
    //public const string Basic =nameof(Basic);
    //public const string Users =nameof(Users);
}
public enum PermissionsEnum : byte
{
    Read,
    Create,
    Update,
    Delete,
    ReadRestricted,
    CreateRestricted,
    UpdateRestricted,
    DeleteRestricted
}