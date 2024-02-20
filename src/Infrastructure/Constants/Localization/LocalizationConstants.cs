// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Constants.Localization;

public static class LocalizationConstants
{
    public const string ResourcesPath = "Resources";

    public static readonly LanguageCode[] SupportedLanguages =
    {
        new()
        {
            Code = "en-US",
            DisplayName = "English"
        },
        new()
        {
            Code = "de-DE",
            DisplayName = "Deutsch"
        },
        new()
        {
            Code = "ru-RU",
            DisplayName = "Russian"
        },
        new()
        {
            Code = "fr-FR",
            DisplayName = "French"
        },
        new()
        {
            Code = "ja-JP",
            DisplayName = "Japanese"
        },
        new()
        {
            Code = "km-KH",
            DisplayName = "Khmer"
        },
        new()
        {
            Code = "ca-ES",
            DisplayName = "Catalan"
        },
        new()
        {
            Code = "es-ES",
            DisplayName = "Spanish"
        },
        new()
        {
            Code = "zh-CN",
            DisplayName = "中文"
        },
        new()
        {
            Code = "ar-iq",
            DisplayName = "Arabic"
        },
        new()
        {
            Code = "ko-kr",
            DisplayName = "Korean"
        }
    };
}

public class LanguageCode
{
    public string DisplayName { get; set; } = "en-US";
    public string Code { get; set; } = "English";
}