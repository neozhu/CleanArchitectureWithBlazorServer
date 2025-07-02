using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Product Permissions")]
    [Description("Set permissions for product operations")]
    public static class Products
    {
        [Description("Allows viewing product details")]
        public const string View = "PermissionsProductsView";

        [Description("Allows creating new product records")]
        public const string Create = "PermissionsProductsCreate";

        [Description("Allows modifying existing product details")]
        public const string Edit = "PermissionsProductsEdit";

        [Description("Allows deleting product records")]
        public const string Delete = "PermissionsProductsDelete";

        [Description("Allows printing product details")]
        public const string Print = "PermissionsProductsPrint";

        [Description("Allows searching for product records")]
        public const string Search = "PermissionsProductsSearch";

        [Description("Allows exporting product records")]
        public const string Export = "PermissionsProductsExport";

        [Description("Allows importing product records")]
        public const string Import = "PermissionsProductsImport";
    }
}


public class ProductsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Print { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
}