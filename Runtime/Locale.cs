using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a locale
  [CreateAssetMenu(menuName = "Audune/Localization/Locale", fileName = "Locale")]
  public sealed class Locale : ScriptableObject, ILocalizedTable<string>, IMessageFormatProvider
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
    public LocalizedStringTable strings => _strings;

    // Return the culture of the locale
    public CultureInfo culture => CultureInfoExtensions.GetCultureInfoOrInvariant(_code);

    // Return a plural rule list for plurals for the locale
    public PluralRuleList pluralRules => PluralRuleDatabase.plurals.TryGetRules(this, out var rules) ? rules : null;

    // Return a plural rule list for ordinal plurals for the locale
    public PluralRuleList ordinalPluralRules => PluralRuleDatabase.ordinalPlurals.TryGetRules(this, out var rules) ? rules : null;


    // Reaturn a message formatter that uses this locale
    internal MessageFormatter CreateFormatter(IMessageFunctionExecutor functionExecutor)
    {
      return new MessageFormatter(this, pluralRules, ordinalPluralRules, functionExecutor, this);
    }

    // Return the string representation of the locale
    public override string ToString()
    {
      return _englishName;
    }


    #region Localized string table implementation
    // Return if an entry in the strings table with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out string value)
    {
      var result = _strings.TryFind(path, out value);
      if (!result)
        Debug.LogWarning($"[LocalizationSystem] Could not find string \"{path}\" in locale {this}");
      return result;
    }

    // Return the value of the entry in the strings table with the specified path, or a default value if one cannot be found
    public string Find(string path, string defaultValue = default)
    {
      if (_strings.TryFind(path, out var value))
        return value;
     
      Debug.LogWarning($"[LocalizationSystem] Could not find string \"{path}\" in locale {this}");
      return defaultValue;
    }
    #endregion

    #region Message format provider implementation
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
  }
}
