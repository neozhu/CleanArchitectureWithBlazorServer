using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{

    [DisplayName("Document Permissions")]
    [Description("Set permissions for document operations")]
    public static class Documents
    {
        [Description("Allows viewing document details")]
        public const string View = "PermissionsDocumentsView";

        [Description("Allows creating new document records")]
        public const string Create = "PermissionsDocumentsCreate";

        [Description("Allows modifying existing document details")]
        public const string Edit = "PermissionsDocumentsEdit";

        [Description("Allows deleting document records")]
        public const string Delete = "PermissionsDocumentsDelete";

        [Description("Allows searching for document records")]
        public const string Search = "PermissionsDocumentsSearch";

        [Description("Allows exporting document records")]
        public const string Export = "PermissionsDocumentsExport";

        [Description("Allows importing document records")]
        public const string Import = "PermissionsDocumentsImport";

        [Description("Allows downloading documents")]
        public const string Download = "PermissionsDocumentsDownload";
    }
}

public class DocumentsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
    public bool Download { get; set; }
}