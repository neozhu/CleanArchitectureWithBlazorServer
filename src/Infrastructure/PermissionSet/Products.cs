using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Product Permissions")]
    [Description("Set permissions for product operations")]
    public static class Products
    {
        [Description("Allows viewing product details")]
        public const string View = "Permissions.Products.View";

        [Description("Allows creating new product records")]
        public const string Create = "Permissions.Products.Create";

        [Description("Allows modifying existing product details")]
        public const string Edit = "Permissions.Products.Edit";

        [Description("Allows deleting product records")]
        public const string Delete = "Permissions.Products.Delete";

        [Description("Allows printing product details")]
        public const string Print = "Permissions.Products.Print";

        [Description("Allows searching for product records")]
        public const string Search = "Permissions.Products.Search";

        [Description("Allows exporting product records")]
        public const string Export = "Permissions.Products.Export";

        [Description("Allows importing product records")]
        public const string Import = "Permissions.Products.Import";
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