// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Infrastructure.Constants.Localization;

public static class LocalizationConstants
{
    public const string ResourcesPath = "Resources";
    public static readonly LanguageCode[] SupportedLanguages = {
            new LanguageCode
            {
                Code = "en-US",
                DisplayName= "English"
            },
            new LanguageCode
            {
                Code = "de-DE",
                DisplayName = "Deutsch"
            },
             new LanguageCode
            {
                Code = "ru",
                DisplayName = "Russian"
            },
             new LanguageCode
            {
                Code = "ja-JP",
                DisplayName = "Japanese"
            },
            new LanguageCode
            {
                Code = "zh-CN",
                DisplayName = "中文"
            }
        };
}

public class LanguageCode
{
    public string DisplayName { get; set; }
    public string Code { get; set; }
}
