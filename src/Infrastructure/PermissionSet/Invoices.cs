

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Invoice Permissions")]
    [Description("Set permissions for invoice operations.")]
    public static class Invoices
    {
        [Description("Allows viewing invoice details.")]
        public const string View = "Permissions.Invoices.View";
        [Description("Allows creating invoice details.")]
        public const string Create = "Permissions.Invoices.Create";
        [Description("Allows editing invoice details.")]
        public const string Edit = "Permissions.Invoices.Edit";
        [Description("Allows deleting invoice details.")]
        public const string Delete = "Permissions.Invoices.Delete";
        [Description("Allows printing invoice details.")]
        public const string Print = "Permissions.Invoices.Print";
        [Description("Allows searching invoice details.")]
        public const string Search = "Permissions.Invoices.Search";
        [Description("Allows exporting invoice details.")]
        public const string Export = "Permissions.Invoices.Export";
        [Description("Allows importing invoice details.")]
        public const string Import = "Permissions.Invoices.Import";
    }
}

