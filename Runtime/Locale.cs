﻿using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Audune.Localization
{
  // Class that defines a locale
  [CreateAssetMenu(menuName = "Audune/Localization/Locale", fileName = "Locale")]
  public sealed class Locale : ScriptableObject, ILocale
  {
    // Constans that define defaults for formats
    public const string defaultDecimalNumberFormat = "n";
    public const string defaultPercentNumberFormat = "p";
    public const string defaultCurrencyNumberFormat = "c";
    public const string defaultShortDateFormat = "d";
    public const string defaultLongDateFormat = "D";
    public const string defaultShortTimeFormat = "t";
    public const string defaultLongTimeFormat = "T";


    // Locale properties
    [SerializeField, Tooltip("The ISO 639 code of the locale")]
    internal string _code;
    [SerializeField, Tooltip("The alternative codes of the locale, in the format"), SerializableDictionaryOptions(keyHeader = "Alternative Code")]
    internal SerializableDictionary<string, string> _altCodes;
    [SerializeField, Tooltip("The English name of the locale")]
    internal string _englishName;
    [SerializeField, Tooltip("The native name of the locale")]
    internal string _nativeName;
    [SerializeField, Tooltip("The decimal number format of the locale")]
    internal string _decimalNumberFormat = defaultDecimalNumberFormat;
    [SerializeField, Tooltip("The percent number format of the locale")]
    internal string _percentNumberFormat = defaultPercentNumberFormat;
    [SerializeField, Tooltip("The currency number format of the locale")]
    internal string _currencyNumberFormat = defaultCurrencyNumberFormat;
    [SerializeField, Tooltip("The short date format of the locale")]
    internal string _shortDateFormat = defaultShortDateFormat;
    [SerializeField, Tooltip("The long date format of the locale")]
    internal string _longDateFormat = defaultLongDateFormat;
    [SerializeField, Tooltip("The short time format of the locale")]
    internal string _shortTimeFormat = defaultShortTimeFormat;
    [SerializeField, Tooltip("The long time format of the locale")]
    internal string _longTimeFormat = defaultLongTimeFormat;

    // Locale tables
    [SerializeField, Tooltip("The strings table of the locale")]
    internal LocalizedStringTable _strings = new LocalizedStringTable();


    // Return the code of the locale
    public string code => _code;

    // Return the English name of the locale
    public string englishName => _englishName;

    // Return the native name of the locale
    public string nativeName => _nativeName;

    // Return the alternative codes of the locale
    public IReadOnlyDictionary<string, string> altCodes => _altCodes;

    // Return the strings table of the locale
    public ILocalizedStringTable strings => _strings;

    // Return the culture of the locale
    public CultureInfo culture => CultureInfoExtensions.GetCultureInfoOrInvariant(_code);


    // Return the string representation of the locale
    public override string ToString()
    {
      return _englishName;
    }


    #region Message format provider implementation
    // Return a plural rule list for plurals for the locale
    public IPluralizer pluralRules => PluralRuleDatabase.plurals.TryGetRules(this, out var rules) ? rules : null;

    // Return a plural rule list for ordinal plurals for the locale
    public IPluralizer ordinalPluralRules => PluralRuleDatabase.ordinalPlurals.TryGetRules(this, out var rules) ? rules : null;

    // Return the localized string table of the formatter
    ILocalizedStringTable IMessageFormatProvider.localizedStringTable => strings;


    // Return the number format for a number format style
    public string GetNumberFormat(NumberFormatStyle style)
    {
      return style switch {
        NumberFormatStyle.Decimal => _decimalNumberFormat,
        NumberFormatStyle.Percent => _percentNumberFormat,
        NumberFormatStyle.Currency => _currencyNumberFormat,
        _ => throw new ArgumentException($"Number format style {style} is unsupported"),
      };
    }

    // Return the date format for a date format style
    public string GetDateFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => _shortDateFormat,
        DateFormatStyle.Long => _longDateFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    // Return the time format for a date format style
    public string GetTimeFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => _shortTimeFormat,
        DateFormatStyle.Long => _longTimeFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    // Return the formatted representation of a number
    public string FormatNumber(NumberContext number, NumberFormatStyle style = NumberFormatStyle.Decimal)
    {
      return number.value.ToString(GetNumberFormat(style), culture);
    }

    // Return the formatted representation of a date
    public string FormatDate(DateTime value, DateFormatStyle style = DateFormatStyle.Short)
    {
      return value.ToString(GetDateFormat(style), culture);
    }

    // Return the formatted representation of a time
    public string FormatTime(DateTime value, DateFormatStyle style = DateFormatStyle.Short)
    {
      return value.ToString(GetTimeFormat(style), culture);
    }
    #endregion

    #region Editor methods
#if UNITY_EDITOR
    // Return all locale assets in the asset database
    public static IEnumerable<Locale> GetAllLocaleAssets(params string[] searchInFolders)
    {
      return AssetDatabase.FindAssets("t:Audune.Localization.Locale", searchInFolders)
        .Select(guid => AssetDatabase.LoadAssetAtPath<Locale>(AssetDatabase.GUIDToAssetPath(guid)))
        .OrderBy(locale => locale.code);
    }
#endif
    #endregion
  }
}
