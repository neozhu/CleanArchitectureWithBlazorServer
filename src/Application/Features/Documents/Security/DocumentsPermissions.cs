// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Document Permissions")]
    [Description("Set permissions for document operations")]
    public static class Documents
    {
        [Description("Allows viewing document details")]
        public const string View = "Permissions.Documents.View";

        [Description("Allows creating new document records")]
        public const string Create = "Permissions.Documents.Create";

        [Description("Allows modifying existing document details")]
        public const string Edit = "Permissions.Documents.Edit";

        [Description("Allows deleting document records")]
        public const string Delete = "Permissions.Documents.Delete";

        [Description("Allows downloading document files")]
        public const string Download = "Permissions.Documents.Download";

        [Description("Allows searching for document records")]
        public const string Search = "Permissions.Documents.Search";

        [Description("Allows exporting document records")]
        public const string Export = "Permissions.Documents.Export";

        [Description("Allows importing document records")]
        public const string Import = "Permissions.Documents.Import";
    }
}

public class DocumentsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Download { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
} 