using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Audune.Localization.Settings.Selectors
{
  // Class that defines a locale selector that uses the system language
  public sealed class SystemLocaleSelector : LocaleSelector
  {
    // Return the locale according to this selector
    public override Locale Select(IReadOnlyList<Locale> locales)
    {
      // Use the current culture to find a locale
      var culture = CultureInfo.CurrentUICulture;
      var selectedLocale = locales.Where(locale => locale.Code == culture.TwoLetterISOLanguageName).FirstOrDefault();

      if (selectedLocale != null) 
        return selectedLocale;

      // If no locale found, then use the application language
      var systemLanguage = Application.systemLanguage;
      if (systemLanguage != SystemLanguage.Unknown)
        selectedLocale = locales.Where(locale => locale.Code == GetSystemLanguageCode(systemLanguage)).FirstOrDefault();

      return selectedLocale;
    }


    // Return the language code for a system language
    public static string GetSystemLanguageCode(SystemLanguage language)
    {
      // TODO: Add SystemLanguage.Hindi when upgrading to Unity 2022
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
