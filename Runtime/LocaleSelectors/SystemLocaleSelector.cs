using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a locale selector that uses the system language
  [AddComponentMenu("Audune/Localization/Locale Selectors/System Locale Selector")]
  public sealed class SystemLocaleSelector : LocaleSelector
  {
    // Locale selector properties
    [SerializeField, Tooltip("The checks to execute to select a locale")]
    private SystemLocaleCheckType _checkTypes = SystemLocaleCheckType.CultureInfo | SystemLocaleCheckType.UnitySystemLanguage;


    // Return if a locale could be selected according to this selector and store the selected locale
    public override bool TrySelectLocale(IReadOnlyList<Locale> locales, out Locale locale)
    {
      locale = null;

      // Use the .NET CultureInfo class to select the locale
      if (_checkTypes.HasFlag(SystemLocaleCheckType.CultureInfo))
      {
        var culture = CultureInfo.CurrentUICulture;
        locale = locales.Where(locale => locale.code == culture.IetfLanguageTag ||  locale.code == culture.TwoLetterISOLanguageName).FirstOrDefault();

        if (locale != null)
          return true;
      }

      // If no locale found, then use the application language
      if (_checkTypes.HasFlag(SystemLocaleCheckType.UnitySystemLanguage))
      {
        var systemLanguage = Application.systemLanguage;
        if (systemLanguage != SystemLanguage.Unknown)
          locale = locales.Where(locale => locale.code == GetSystemLanguageCode(systemLanguage)).FirstOrDefault();

        if (locale != null)
          return true;
      }

      // No locale selected
      return false;
    }


    // Return the language code for a system language
    public static string GetSystemLanguageCode(SystemLanguage language)
    {
      return language switch {
        SystemLanguage.Afrikaans => "af",
        SystemLanguage.Arabic => "ar",
        SystemLanguage.Basque => "eu",
        SystemLanguage.Belarusian => "be",
        SystemLanguage.Bulgarian => "bg",
        SystemLanguage.Catalan => "ca",
        SystemLanguage.Chinese => "zh-CN",
        SystemLanguage.ChineseSimplified => "zh-Hans",
        SystemLanguage.ChineseTraditional => "zh-Hant",
        SystemLanguage.Czech => "cs",
        SystemLanguage.Danish => "da",
        SystemLanguage.Dutch => "nl",
        SystemLanguage.English => "en",
        SystemLanguage.Estonian => "et",
        SystemLanguage.Faroese => "fo",
        SystemLanguage.Finnish => "fi",
        SystemLanguage.French => "fr",
        SystemLanguage.German => "de",
        SystemLanguage.Greek => "el",
        SystemLanguage.Hebrew => "he",
        SystemLanguage.Hindi => "hi",
        SystemLanguage.Hungarian => "hu",
        SystemLanguage.Icelandic => "is",
        SystemLanguage.Indonesian => "id",
        SystemLanguage.Italian => "it",
        SystemLanguage.Japanese => "ja",
        SystemLanguage.Korean => "ko",
        SystemLanguage.Latvian => "lv",
        SystemLanguage.Lithuanian => "lt",
        SystemLanguage.Norwegian => "no",
        SystemLanguage.Polish => "pl",
        SystemLanguage.Portuguese => "pt",
        SystemLanguage.Romanian => "ro",
        SystemLanguage.Russian => "ru",
        SystemLanguage.SerboCroatian => "hr",
        SystemLanguage.Slovak => "sk",
        SystemLanguage.Slovenian => "sl",
        SystemLanguage.Spanish => "es",
        SystemLanguage.Swedish => "sv",
        SystemLanguage.Thai => "th",
        SystemLanguage.Turkish => "tr",
        SystemLanguage.Ukrainian => "uk",
        SystemLanguage.Vietnamese => "vi",
        _ => null,
      };
    }
  }
}
