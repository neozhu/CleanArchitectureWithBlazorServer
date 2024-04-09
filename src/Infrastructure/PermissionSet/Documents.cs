using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Documents")]
    [Description("Documents Permissions")]
    public static class Documents
    {
        public const string View = "Permissions.Documents.View";
        public const string Create = "Permissions.Documents.Create";
        public const string Edit = "Permissions.Documents.Edit";
        public const string Delete = "Permissions.Documents.Delete";
        public const string Search = "Permissions.Documents.Search";
        public const string Export = "Permissions.Documents.Export";
        public const string Import = "Permissions.Documents.Import";
        public const string Download = "Permissions.Documents.Download";
    }
}