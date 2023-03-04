using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Constants;
public static class ConstantString
{
    //==========================================================//
    //for button text
    public static string REFRESH => Localize("Refresh");
    public static string EDIT => Localize("Edit");
    public static string SUBMIT => Localize("Submit");
    public static string DELETE => Localize("Delete");
    public static string ADD => Localize("Add");
    public static string CREATE => Localize("Create");
    public static string EXPORT => Localize("Export to Excel");
    public static string EXPORTPDF => Localize("Export to PDF");
    public static string IMPORT => Localize("Import from Excel");
    public static string ACTIONS => Localize("Actions");
    public static string SAVE => Localize("Save");
    public static string SAVECHANGES => Localize("Save Changes");
    public static string CANCEL => Localize("Cancel");
    public static string CLOSE => Localize("Close");
    public static string SEARCH => Localize("Search");
    public static string CLEAR => Localize("Clear");
    public static string RESET => Localize("Reset");
    public static string OK => Localize("OK");
    public static string CONFIRM => Localize("Confirm");
    public static string YES => Localize("Yes");
    public static string NO => Localize("No");
    public static string NEXT => Localize("Next");
    public static string PREVIOUS => Localize("Previous");
    public static string UPLOAD => Localize("Upload");
    public static string DOWNLOAD => Localize("Download");
    public static string UPLOADING => Localize("Uploading...");
    public static string DOWNLOADING => Localize("Downloading...");
    public static string NOALLOWED => Localize("No Allowed");
    public static string SIGNINWITH => Localize("Sign in with {0}");
    public static string LOGOUT => Localize("Logout");
    public static string SIGNIN => Localize("Sign In");
    public static string Microsoft => Localize("Microsoft");
    public static string Facebook => Localize("Facebook");
    public static string Google => Localize("Google");

    //============================================================================//
    // for toast message
    public static string SAVESUCCESS => Localize("Save successfully");
    public static string DELETESUCCESS => Localize("Delete successfully");
    public static string DELETEFAIL => Localize("Delete fail");
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

    //========================================================

    public static string ADVANCEDSEARCH => Localize("Advanced Search");
    public static string ORDERBY => Localize("Order By");
    public static string CREATEAITEM => Localize("Create a new {0}");
    public static string EDITTHEITEM => Localize("Edit the {0}");
    public static string DELETETHEITEM => Localize("Delete the {0}");
    public static string DELETEITEMS => Localize("Delete selected items: {0}");
    public static string DELETECONFIRMATION => Localize("Are you sure you want to delete this item: {0}?");
    public static string DELETECONFIRMATIONWITHID => Localize("Are you sure you want to delete this item with Id: {0}?");
    public static string DELETECONFIRMWITHSELECTED => Localize("Are you sure you want to delete the selected items: {0}?");
    public static string NOMACHING => Localize("No matching records found");
    public static string LOADING => Localize("Loading...");
    public static string WATING => Localize("Wating...");
    public static string PROCESSING => Localize("Processing...");
    public static string DELETECONFIRMATIONTITLE => Localize("Delete Confirmation");
    public static string LOGOUTCONFIRMATIONTITLE => Localize("Logout Confirmation");
    public static string LOGOUTCONFIRMATION => Localize("You are attempting to log out of application. Do you really want to log out?");
}
