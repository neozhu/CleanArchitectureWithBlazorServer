
using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("OfferLine Permissions")]
    [Description("Set permissions for offerline operations.")]
    public static class OfferLines
    {
        [Description("Allows viewing offerline details.")]
        public const string View = "Permissions.OfferLines.View";
        [Description("Allows creating offerline details.")]
        public const string Create = "Permissions.OfferLines.Create";
        [Description("Allows editing offerline details.")]
        public const string Edit = "Permissions.OfferLines.Edit";
        [Description("Allows deleting offerline details.")]
        public const string Delete = "Permissions.OfferLines.Delete";
        [Description("Allows printing offerline details.")]
        public const string Print = "Permissions.OfferLines.Print";
        [Description("Allows searching offerline details.")]
        public const string Search = "Permissions.OfferLines.Search";
        [Description("Allows exporting offerline details.")]
        public const string Export = "Permissions.OfferLines.Export";
        [Description("Allows importing offerline details.")]
        public const string Import = "Permissions.OfferLines.Import";
    }
}

