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
            DisplayName = "English (United States)"
        },
        new()
        {
            Code = "ar-iq",
            DisplayName = "Arabic"
        },
        new()
        {
            Code = "de-DE",
            DisplayName = "Deutsch (Deutschland)"
        },
        new()
        {
            Code = "ru-RU",
            DisplayName = "русский (Россия)"
        },
        new()
        {
            Code = "fr-FR",
            DisplayName = "français (France)"
        },
        new()
        {
            Code = "ja-JP",
            DisplayName = "日本語 (日本)"
        },
        new()
        {
            Code = "km-KH",
            DisplayName = "ខ្មែរ (កម្ពុជា)"
        },
        new()
        {
            Code = "ca-ES",
            DisplayName = "català (Espanya)"
        },
        new()
        {
            Code = "es-ES",
            DisplayName = "español (España)"
        },
        new()
        {
            Code = "zh-CN",
            DisplayName = "中文（简体，中国）"
        },
        new()
        {
            Code = "ko-kr",
            DisplayName = "한국어(대한민국)"
        }
    };
}

public class LanguageCode
{
    public string DisplayName { get; set; } = "en-US";
    public string Code { get; set; } = "English";
}