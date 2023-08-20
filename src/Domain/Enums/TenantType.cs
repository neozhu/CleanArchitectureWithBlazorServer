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

    Pharmacy = 11,
    DiagnosticCenter = 14,

    Hospital = 17,

    Internal = 20
}
