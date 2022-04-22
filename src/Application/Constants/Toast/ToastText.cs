using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Constants;
public static class ToastText
{
    public static string SAVESUCCESS => Localize("Save successfully");
    public static string DELETESUCCESS => Localize("Delete successfully");
    public static string UPDATESUCCESS => Localize("Update successfully");
    public static string CREATESUCCESS => Localize("Create successfully");
    public static string LOGINSUCCESS => Localize("Login successfully");
    public static string LOGOUTSUCCESS => Localize("Logout successfully");
    public static string LOGINFAIL => Localize("Login fail");
    public static string LOGOUTFAIL => Localize("Logout fail");
    public static string IMPORTSUCCESS => Localize("Import successfully");
    public static string IMPORTFAIL => Localize("Import fail");
    public static string EXPORTSUCESS => Localize("Export successfully");
    public static string EXPORTFAIL => Localize("Export fail");
    public static string UPLOADSUCCESS => Localize("Upload successfully");
}
