using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Product Permissions")]
    [Description("Set permissions for product operations.")]
    public static class Products
    {

        [Description("Allows viewing product details.")]
        public const string View = "Permissions.Products.View";

        [Description("Allows creating product details.")]
        public const string Create = "Permissions.Products.Create";

        [Description("Allows editing product details.")]
        public const string Edit = "Permissions.Products.Edit";

        [Description("Allows deleting product details.")]
        public const string Delete = "Permissions.Products.Delete";

        [Description("Allows print product details.")]
        public const string Print = "Permissions.Products.Print";

        [Description("Allows searching product details.")]
        public const string Search = "Permissions.Products.Search";

        [Description("Allows exporting product details.")]
        public const string Export = "Permissions.Products.Export"; 

        [Description("Allows importing product details.")]
        public const string Import = "Permissions.Products.Import";
    }



    [DisplayName("Category Permissions")]
    [Description("Set permissions for category operations.")]
    public static class Categories
    {
        public const string View = "Permissions.Categories.View";
        public const string Create = "Permissions.Categories.Create";
        public const string Edit = "Permissions.Categories.Edit";
        public const string Delete = "Permissions.Categories.Delete";
        public const string Search = "Permissions.Categories.Search";
        public const string Export = "Permissions.Categories.Export";
        public const string Import = "Permissions.Categories.Import";
    }
}