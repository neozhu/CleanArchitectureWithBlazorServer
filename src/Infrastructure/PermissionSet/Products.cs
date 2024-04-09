using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Products")]
    [Description("Products Permissions")]
    public static class Products
    {
        public const string View = "Permissions.Products.View";
        public const string Create = "Permissions.Products.Create";
        public const string Edit = "Permissions.Products.Edit";
        public const string Delete = "Permissions.Products.Delete";
        public const string Search = "Permissions.Products.Search";
        public const string Export = "Permissions.Products.Export";
        public const string Import = "Permissions.Products.Import";
    }


    [DisplayName("Categories")]
    [Description("Categories Permissions")]
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