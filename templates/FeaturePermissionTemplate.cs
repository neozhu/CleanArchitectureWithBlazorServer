// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

/// <summary>
/// 为新模块添加权限定义的模板
/// 使用方式：
/// 1. 复制此文件到 Features/YourModule/Security/ 目录
/// 2. 将 {{ModuleName}} 替换为您的模块名称
/// 3. 根据需要添加或删除权限
/// 4. 更新 AccessRights 类中的属性
/// </summary>
public static partial class Permissions
{
    [DisplayName("{{ModuleName}} Permissions")]
    [Description("Set permissions for {{ModuleName}} operations")]
    public static class {{ModuleName}}s
    {
        [Description("Allows viewing {{ModuleName}} details")]
        public const string View = "Permissions.{{ModuleName}}s.View";

        [Description("Allows creating new {{ModuleName}} records")]
        public const string Create = "Permissions.{{ModuleName}}s.Create";

        [Description("Allows modifying existing {{ModuleName}} details")]
        public const string Edit = "Permissions.{{ModuleName}}s.Edit";

        [Description("Allows deleting {{ModuleName}} records")]
        public const string Delete = "Permissions.{{ModuleName}}s.Delete";

        [Description("Allows searching for {{ModuleName}} records")]
        public const string Search = "Permissions.{{ModuleName}}s.Search";

        [Description("Allows exporting {{ModuleName}} records")]
        public const string Export = "Permissions.{{ModuleName}}s.Export";

        [Description("Allows importing {{ModuleName}} records")]
        public const string Import = "Permissions.{{ModuleName}}s.Import";

        // 根据需要添加更多权限
        // [Description("Allows printing {{ModuleName}} details")]
        // public const string Print = "Permissions.{{ModuleName}}s.Print";
    }
}

/// <summary>
/// {{ModuleName}} 访问权限类，用于权限检查
/// </summary>
public class {{ModuleName}}sAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
    
    // 根据需要添加更多权限属性
    // public bool Print { get; set; }
} 