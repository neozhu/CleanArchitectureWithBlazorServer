// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Tracing;
using System.Numerics;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Vml;
using Microsoft.CodeAnalysis;

namespace CleanArchitecture.Blazor.Application.Constants.Role;

public enum RolesEnum : byte
{
    RootAdmin=50,
    ElevateAdminGroup=45,
    ElevateAdminViewers=41,

    Hospital=30,
    HospitalAdmin=29,
    DoctorHOD=28,
    Doctor=27,
    DoctorAssistant=26,
    Nurse=25,
    ViewerHospital=21,

    DiagnosticCenter=20,
    Diagnostics=16,

    Pharmacy=15,
    Pharmacists=11,

    Patient=4,
    
    [Description(nameof(Guest))]
    Guest = 1,
    Rejected = 0,
    // public const string Guest = nameof(Guest);//vmadhu2023 =>requesting for Hospital registration , vmadhu203=>patient   ,   vmadhu2022 =>req for doctor 
    DefaultRole1 = Patient
    //public const string Basic =nameof(Basic);
    //public const string Users =nameof(Users);
}
public abstract class RoleName
{
    public const string RootAdmin = nameof(RootAdmin);  //madhu.veer
    public const string ElevateAdminGroup = nameof(ElevateAdminGroup);
    public const string ElevateAdminViewers = nameof(ElevateAdminViewers);

    public const string Hospital = nameof(Hospital);
    public const string HospitalAdmin = nameof(HospitalAdmin);
    public const string DoctorHOD = nameof(DoctorHOD);
    public const string Doctor = nameof(Doctor);
    public const string DoctorAssistant = nameof(DoctorAssistant);
    public const string Nurse = nameof(Nurse);
    public const string ViewerHospital = nameof(ViewerHospital);

    public const string DiagnosticCenter = nameof(DiagnosticCenter);
    public const string Diagnostics = nameof(Diagnostics);

    public const string Pharmacy = nameof(Pharmacy);
    public const string Pharmacists = nameof(Pharmacists);

    public const string Patient = nameof(Patient);

    // public const string Guest = nameof(Guest);//vmadhu2023 =>requesting for Hospital registration , vmadhu203=>patient   ,   vmadhu2022 =>req for doctor 
    public const string DefaultRole1 = nameof(Patient);
    //public const string Basic =nameof(Basic);
    //public const string Users =nameof(Users);

}
