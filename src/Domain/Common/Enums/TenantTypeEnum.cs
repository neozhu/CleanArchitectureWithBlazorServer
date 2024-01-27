using System.ComponentModel;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum TenantTypeEnum : byte
{//mORE IS HIGHEST PREVILEGE
    Rejected = 0,
    [Description(nameof(Guest))]
    Guest = 1,

    [Description(nameof(Patient))]
    Patient = 4,

    [Description("New Request for creating HOSPITAL/PHARMACY/DIAGNOSTICCENTER")]
    NewTenant = 6,

    [Description("PHARMACY & Staff related")]
    PharmacyAndStaff = 15,

    [Description("DIAGNOSTICIAN center & Staff related")]
    DiagnosticsAndStaff = 20,

    [Description("HOSPITAL & All Staff related")]
    HospitalAndStaff = 30,

    Internal = 50
}
