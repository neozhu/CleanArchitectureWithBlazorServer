using System.ComponentModel;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum TenantType : byte
{
    Rejected = 0,
    [Description(nameof(Guest))]
    Guest = 1,

    [Description(nameof(Patient))]
    Patient = 4,

    [Description("New Request for creating Hospital/Pharmacy/DiagnosticCenter")]
    NewTenant = 6,

    PharmacyAndStaff = 11,
    DiagnosticsAndStaff = 14,

    [Description("Hospital And All Staff")]
    HospitalAndStaff = 17,

    Internal = 20
}
