// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Constants;

public static class LocalizationConstants
{
    public const string ResourcesPath = "Resources";
    /// <summary>
    /// Default language code. Set to English (en-US). 
    /// Change to "zh-CN" for Chinese or other codes from SupportedLanguages.
    /// </summary>
    public const string DefaultLanguageCode = "en-US";

    public static readonly LanguageCode[] SupportedLanguages =
    {
        new()
        {
            Code = "en-US",
            DisplayName = "English (United States)"
        },
        new()
        {
            Code = "de-DE",
            DisplayName = "Deutsch (Deutschland)"
        },
        new()
        {
            Code = "zh-CN",
            DisplayName = "中文（简体，中国）"
        },
 
    };
}

public class LanguageCode
{
    public string DisplayName { get; set; } = "en-US";
    public string Code { get; set; } = "English";
} 
