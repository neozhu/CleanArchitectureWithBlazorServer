// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Constants.Localization;

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
                Code = "fr-FR",
                DisplayName = "French"
            },
            new LanguageCode
            {
                Code = "ja-JP",
                DisplayName = "Japanese"
            },
            new LanguageCode
            {
                Code = "km-KH",
                DisplayName = "Khmer"
            },
            new LanguageCode
            {
                Code = "ca-ES",
                DisplayName = "Catalan"
            },
            new LanguageCode
            {
                Code = "es-ES",
                DisplayName = "Spanish"
            },
            new LanguageCode
            {
                Code = "zh-CN",
                DisplayName = "中文"
            },
            new LanguageCode
            {
                Code = "ar-iq",
                DisplayName = "Arabic Iraq"
            }
        };
}

public class LanguageCode
{
    public string DisplayName { get; set; } = "en-US";
    public string Code { get; set; } = "English";
}
