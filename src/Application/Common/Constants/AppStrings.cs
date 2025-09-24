using System.Globalization;
using System.Resources;

namespace CleanArchitecture.Blazor.Application.Common.Constants;

public static class AppStrings
{
    public const string APPSTRINGS_RESOURCE_ID =
        "CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings";

    private static readonly ResourceManager rm;

    static AppStrings()
    {
        rm = new ResourceManager(APPSTRINGS_RESOURCE_ID, typeof(AppStrings).Assembly);
    }

    //==========================================================//
    //for button text
     public static string ListView => Localize("List View");
    public static string More => Localize("More");
    public static string Print => Localize("Print");
    public static string GoBack => Localize("Back");
    public static string Refresh => Localize("Refresh");
    public static string Edit => Localize("Edit");
    public static string Submit => Localize("Submit");
    public static string Delete => Localize("Delete");
    public static string Add => Localize("Add");
    public static string Clone => Localize("Clone");
    public static string New => Localize("New");
    public static string Export => Localize("Export to Excel");
    public static string ExportPDF => Localize("Export to PDF");
    public static string Import => Localize("Import from Excel");
    public static string Actions => Localize("Actions");
    public static string Save => Localize("Save");
    public static string SaveAndNew => Localize("Save & New");
    public static string SaveChanges => Localize("Save Changes");
    public static string Cancel => Localize("Cancel");
    public static string Close => Localize("Close");
    public static string Search => Localize("Search");
    public static string Clear => Localize("Clear");
    public static string Reset => Localize("Reset");
    public static string Ok => Localize("OK");
    public static string Confirm => Localize("Confirm");
    public static string Yes => Localize("Yes");
    public static string No => Localize("No");
    public static string Next => Localize("Next");
    public static string Previous => Localize("Previous");
    public static string Upload => Localize("Upload");
    public static string Download => Localize("Download");
    public static string Uploading => Localize("Uploading...");
    public static string Downloading => Localize("Downloading...");
    public static string Selected => Localize("Selected");
    public static string SelectedTotal => Localize("Selected Total");
    public static string Loading => Localize("Loading...");
    public static string Waiting => Localize("Waiting...");

    public static string NoRecords => Localize("No records found");
    public static string ExportSuccess => Localize("Export successfully");
    public static string ImportSuccess => Localize("Import successfully");
    public static string ExportPDFSuccess => Localize("Export to PDF successfully");
    public static string DeleteConfirmation => Localize("Are you sure you want to delete \"{0}\"?");
    public static string DeleteConfirmationTitle => Localize("Delete Confirmation");
    public static string DeleteConfirmWithSelected => Localize("Are you sure you want to delete {0} selected items?");
   public static string DeleteTheItem => Localize("Delete the item");
    public static string DeleteConfirmationWithId => Localize("Are you sure you want to delete the item with ID: {0}?");
    public static string NoAllowed => Localize("No permission");
    public static string AllowedEdit => Localize("Allow edit");
    public static string AllowedDelete => Localize("Allow delete");
    public static string AllowedCreate => Localize("Allow create");
    public static string AllowedView => Localize("Allow view");
    public static string FilterOptions => Localize("Filter Options");
    public static string AdvancedSearch => Localize("Advanced Search");
    public static string QuickFilter => Localize("Quick Filter");
    public static string Active => Localize("Active");
    public static string Inactive => Localize("Inactive");
    public static string Locked => Localize("Locked");
    public static string Unlocked => Localize("Unlocked");
    public static string Enable => Localize("Enable");
    public static string Disable => Localize("Disable");
    public static string Lock => Localize("Lock");
    public static string Unlock => Localize("Unlock");
    public static string Send => Localize("Send");
    public static string Activate => Localize("Activate");
    public static string Deactivate => Localize("Deactivate");
    
    public static string CreatedAt => Localize("CreatedAt");
    public static string CreatedBy => Localize("CreatedBy");
    public static string LastModifiedAt => Localize("LastModifiedAt");
    public static string LastModifiedBy => Localize("LastModifiedBy");
 
    // Success/Failure messages
    public static string SaveSuccess => Localize("Save successfully");
    public static string CreateSuccess => Localize("Create successfully");
    public static string DeleteSuccess => Localize("Delete successfully");
    public static string UploadSuccess => Localize("Upload successfully");
    public static string ExportFail => Localize("Export failed");
    public static string ImportFail => Localize("Import failed");
    
    

    private static string Localize(string key)
    {
        try
        {
            // Try to get localized string using current UI culture
            var localizedString = rm.GetString(key, CultureInfo.CurrentUICulture);
            
            // If localized string is found and not empty, return it
            if (!string.IsNullOrEmpty(localizedString))
            {
                return localizedString;
            }
            
            // If not found in current culture, try using invariant culture as fallback
            localizedString = rm.GetString(key, CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(localizedString))
            {
                return localizedString;
            }
        }
        catch (Exception)
        {
            // If an exception occurs, fallback to returning the key
            // This may happen when resource files are missing or corrupted
        }
        
        // Final fallback: return the key itself
        return key;
    }
} 
