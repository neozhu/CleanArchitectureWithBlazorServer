using System.ComponentModel;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum TenantTypeEnum : byte
{
    Rejected = 0,
    [Description(nameof(Guest))]
    Guest = 1,

    [Description(nameof(Patient))]
    Patient = 4,

    [Description("New Request for creating Hospital/Pharmacy/DiagnosticCenter")]
    NewTenant = 6,

    [Description("Pharmacy & Staff related")]
    PharmacyAndStaff = 15,

    [Description("Diagnostic center & Staff related")]
    DiagnosticsAndStaff = 20,

    [Description("Hospital & All Staff related")]
    HospitalAndStaff = 30,

    Internal = 50
}
