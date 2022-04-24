using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Constants;
public static class PromptText
{
    public static string ADVANCEDSEARCH => Localize("Advanced Search");
    public static string ORDERBY => Localize("Order By");
    public static string ACTIONS => Localize("Actions");
    public static string SEARCH => Localize("Search");
    public static string CREATEAITEM => Localize("Create a new {0}");
    public static string EDITTHEITEM => Localize("Edit the {0}");
    public static string DELETETHEITEM => Localize("Delete the {0}");
    public static string DELETEITEMS => Localize("Delete selected items: {0}");
    public static string DELETECONFIRMATION => Localize("Are you sure you want to delete this item: {0}?");
    public static string DELETECONFIRMATIONWITHID => Localize("Are you sure you want to delete this item with Id: {0}?");
    public static string DELETECONFIRMWITHSELECTED => Localize("Are you sure you want to delete the selected items: {0}?");
    public static string NOMACHING => Localize("No matching records found");
    public static string LOADING => Localize("Loading...");

    public static string DELETECONFIRMATIONTITLE => Localize("Delete Confirmation");

    public static string LOGOUTCONFIRMATIONTITLE=>Localize("Logout Confirmation");
    public static string LOGOUTCONFIRMATION => Localize("You are attempting to log out of application. Do you really want to log out?");

}
