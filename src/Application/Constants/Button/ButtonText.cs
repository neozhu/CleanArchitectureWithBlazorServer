using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Helper;

namespace CleanArchitecture.Blazor.Application.Constants;

public static class ButtonText
{
    
    public static string REFRESH => Localize("Refresh");
    public static string EDIT => Localize("Edit");
    public static string DELETE => Localize("Delete");
    public static string ADD => Localize("Add");
    public static string CREATE => Localize("Create");
    public static string EXPORT => Localize("Export to Excel");
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
    public static string UPLOADING => Localize("Uploading...");
    public static string DOWNLOADING => Localize("Downloading...");

    public static string NOALLOWED => Localize("No Allowed");

    public static string SIGNINWITH => Localize("Sign in with {0}");
    public static string LOGOUT => Localize("Logout");
    public static string SIGNIN => Localize("Sign In");
    public static string Microsoft => Localize("Microsoft");
    public static string Facebook => Localize("Facebook");
    public static string Google => Localize("Google");
}
