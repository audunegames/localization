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
    [SerializeField, Tooltip("The pluralization rules of the locale"), SerializableDictionaryOptions(keyHeader = "Plural Keyword")]
    internal SerializableDictionary<PluralKeyword, string> _pluralRules;

    // Locale tables
    [SerializeField, Tooltip("The strings table of the locale")]
    internal LocalizedStringTable _strings = new LocalizedStringTable();


    // Return the code of the locale
    public string code => _code;

    // Return the alternative codes of the locale
    public IReadOnlyDictionary<string, string> altCodes => _altCodes;

    // Return the English name of the locale
    public string englishName => _englishName;

    // Return the native name of the locale
    public string nativeName => _nativeName;

    // Return the pluralization rules of the locale
    public PluralRules pluralRules => new PluralRules(_pluralRules);

    // Return the strings of the locale
    public LocalizedStringTable strings => _strings;


    // Return the culture of the locale
    public CultureInfo culture {
      get {
        try
        {
          return CultureInfo.GetCultureInfo(_code);
        }
        catch (Exception ex) when (ex is CultureNotFoundException || ex is ArgumentException)
        {
          return CultureInfo.InvariantCulture;
        }
      }
    }

    // Return a message formatter for the locale
    public MessageFormatter formatter => new MessageFormatter(this, pluralRules);


    


    // Return the string representation of the locale
    public override string ToString()
    {
      return _nativeName;
    }



    // Return the default number format for a number format style
    public static string GetDefaultNumberFormat(NumberFormatStyle style)
    {
      return style switch {
        NumberFormatStyle.Decimal => defaultDecimalNumberFormat,
        NumberFormatStyle.Percent => defaultPercentNumberFormat,
        NumberFormatStyle.Currency => defaultCurrencyNumberFormat,
        _ => throw new ArgumentException($"Number format style {style} is unsupported"),
      };
    }

    // Return the default date format for a date format style
    public static string GetDefaultDateFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => defaultShortDateFormat,
        DateFormatStyle.Long => defaultLongDateFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    // Return the default time format for a date format style
    public static string GetDefaultTimeFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => defaultShortTimeFormat,
        DateFormatStyle.Long => defaultLongTimeFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    #region Localized string table implementation
    // Return if an entry in the strings table with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out string value)
    {
      return _strings.TryFind(path, out value);
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


    // Return the formatted representation of a number as an integer
    public string FormatNumber(int value, NumberFormatStyle style = NumberFormatStyle.Decimal)
    {
      try
      {
        return value.ToString(GetNumberFormat(style), culture);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultNumberFormat(style), culture);
      }
    }

    // Return the formatted representation of a number as a float
    public string FormatNumber(float value, NumberFormatStyle style = NumberFormatStyle.Decimal)
    {
      try
      {
        return value.ToString(GetNumberFormat(style), culture);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultNumberFormat(style), culture);
      }
    }

    // Return the formatted representation of a date
    public string FormatDate(DateTime value, DateFormatStyle style = DateFormatStyle.Short)
    {
      try
      {
        return value.ToString(GetDateFormat(style), culture);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultDateFormat(style), culture);
      }
    }

    // Return the formatted representation of a time
    public string FormatTime(DateTime value, DateFormatStyle style = DateFormatStyle.Short)
    {
      try
      {
        return value.ToString(GetTimeFormat(style), culture);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultTimeFormat(style), culture);
      }
    }
    #endregion
  }
}
