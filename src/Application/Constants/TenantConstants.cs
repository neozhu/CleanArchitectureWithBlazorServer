using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Constants;
public abstract class Constants
{
    public static List<Tenant> DefaultTenants=new() {new("Patient", "Patient"),
    new("HospitalAndStaff","Hospital And All Staff"),new("PharmacyAndStaff","Pharmacy And All Staff"),new("DiagnosticsAndStaff","Diagnostic Center And All Staff")
    ,new("Internal","Internal"),new("NewRegistration","New Registration of Hospital/Pharmacy/Diagnostics")};
}
