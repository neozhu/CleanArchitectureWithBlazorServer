using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum RoleNamesEnum : byte
{//mORE IS HIGHEST PREVILEGE
    ROOTADMIN = 50,
    ELEVATEADMINGROUP = 45,
    ELEVATEADMINVIEWER = 41,

    HOSPITAL = 30,
    HOSPITALADMIN = 29,
    DOCTORHOD = 28,
    DOCTOR = 27,
    DOCTORASSISTANT = 26,
    NURSE = 25,
    VIEWERHOSPITAL = 21,

    DIAGNOSTICCENTER = 20,
    DIAGNOSTICIAN = 16,

    PHARMACY = 15,
    PHARMACIST = 11,

    PATIENT = 4,

    [Description(nameof(GUEST))]
    GUEST = 1,
    REJECTED = 0,
    // public const string GUEST = nameof(GUEST);//vmadhu2023 =>requesting for HOSPITAL registration , vmadhu203=>patient   ,   vmadhu2022 =>req for doctor 
   // DefaultRole1 = PATIENT
    //public const string Basic =nameof(Basic);
    //public const string Users =nameof(Users);
}
public enum PermissionsEnum : byte
{
    Assign,
    UnAssign,
    Read,
    Create,
    Update,
    Delete,
    ReadRestricted,
    CreateRestricted,
    UpdateRestricted,
    DeleteRestricted
}
