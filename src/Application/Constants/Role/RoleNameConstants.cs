// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DocumentFormat.OpenXml.Math;

namespace CleanArchitecture.Blazor.Application.Constants.Role;

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
