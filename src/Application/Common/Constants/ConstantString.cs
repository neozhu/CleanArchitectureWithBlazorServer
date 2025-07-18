using System.Globalization;
using System.Resources;

namespace CleanArchitecture.Blazor.Application.Common.Constants;

public static class ConstantString
{
    public const string CONSTANTSTRINGRESOURCEID =
        "CleanArchitecture.Blazor.Application.Resources.Constants.ConstantString";

    private static readonly ResourceManager rm;

    static ConstantString()
    {
        rm = new ResourceManager(CONSTANTSTRINGRESOURCEID, typeof(ConstantString).Assembly);
    }

    //==========================================================//
    //for button text
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
    public static string NoRecords => Localize("No records found");
    public static string ExportSuccess => Localize("Export successfully");
    public static string ImportSuccess => Localize("Import successfully");
    public static string ExportPDFSuccess => Localize("Export to PDF successfully");
    public static string DeleteConfirmation => Localize("Are you sure you want to delete \"{0}\"?");
    public static string DeleteConfirmationTitle => Localize("Delete Confirmation");
    public static string DeleteConfirmWithSelected => Localize("Are you sure you want to delete {0} selected items?");
    public static string CreateAnItem => Localize("Create a new {0}");
    public static string EditTheItem => Localize("Edit the {0}");
    public static string ViewTheItem => Localize("View the {0}");
    public static string ItemHasBeenCreated => Localize("{0} has been created");
    public static string ItemHasBeenUpdated => Localize("{0} has been updated");
    public static string ItemHasBeenDeleted => Localize("{0} has been deleted");
    public static string ItemHasBeenSaved => Localize("{0} has been saved");
    public static string NoAllowed => Localize("No permission");
    public static string AllowedEdit => Localize("Allow edit");
    public static string AllowedDelete => Localize("Allow delete");
    public static string AllowedCreate => Localize("Allow create");
    public static string AllowedView => Localize("Allow view");
    public static string FilterOptions => Localize("Filter Options");
    public static string AdvancedSearch => Localize("Advanced Search");
    public static string QuickFilter => Localize("Quick Filter");
    public static string ResetPassword => Localize("Reset Password");
    public static string ChangePassword => Localize("Change Password");
    public static string CurrentPassword => Localize("Current Password");
    public static string NewPassword => Localize("New Password");
    public static string ConfirmPassword => Localize("Confirm Password");
    public static string PasswordHasBeenChanged => Localize("Password has been changed");
    public static string PasswordHasBeenReset => Localize("Password has been reset");
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
    public static string Assign => Localize("Assign");
    public static string Unassign => Localize("Unassign");
    public static string Profile => Localize("Profile");
    public static string Settings => Localize("Settings");
    public static string Logout => Localize("Logout");
    public static string Login => Localize("Login");
    public static string Register => Localize("Register");
    public static string ForgotPassword => Localize("Forgot Password");
    public static string RememberMe => Localize("Remember me");
    public static string EmailAddress => Localize("Email Address");
    public static string PhoneNumber => Localize("Phone Number");
    public static string UserName => Localize("User Name");
    public static string Password => Localize("Password");
    public static string FirstName => Localize("First Name");
    public static string LastName => Localize("Last Name");
    public static string FullName => Localize("Full Name");
    public static string Title => Localize("Title");
    public static string Description => Localize("Description");
    public static string Name => Localize("Name");
    public static string Status => Localize("Status");
    public static string Role => Localize("Role");
    public static string Roles => Localize("Roles");
    public static string Permission => Localize("Permission");
    public static string Permissions => Localize("Permissions");
    public static string User => Localize("User");
    public static string Users => Localize("Users");
    public static string Group => Localize("Group");
    public static string Groups => Localize("Groups");
    public static string Department => Localize("Department");
    public static string Departments => Localize("Departments");
    public static string Organization => Localize("Organization");
    public static string Organizations => Localize("Organizations");
    public static string Created => Localize("Created");
    public static string CreatedBy => Localize("Created By");
    public static string CreatedOn => Localize("Created On");
    public static string Modified => Localize("Modified");
    public static string ModifiedBy => Localize("Modified By");
    public static string ModifiedOn => Localize("Modified On");
    public static string System => Localize("System");
    public static string Admin => Localize("Admin");
    public static string SuperAdmin => Localize("Super Admin");
    public static string Basic => Localize("Basic");
    
    // Success/Failure messages
    public static string SaveSuccess => Localize("Save successfully");
    public static string CreateSuccess => Localize("Create successfully");
    public static string DeleteSuccess => Localize("Delete successfully");
    public static string UploadSuccess => Localize("Upload successfully");
    public static string ExportFail => Localize("Export failed");
    public static string ImportFail => Localize("Import failed");
    
    // Additional messages
    public static string Waiting => Localize("Waiting...");
    public static string DeleteTheItem => Localize("Delete the item");
    public static string DeleteConfirmationWithId => Localize("Are you sure you want to delete the item with ID: {0}?");

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