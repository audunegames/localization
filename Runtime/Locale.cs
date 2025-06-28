using Audune.Utils.Dictionary;
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
  /// <summary>
  /// Class that defines a locale.
  /// </summary>
  [CreateAssetMenu(menuName = "Audune/Localization/Locale", fileName = "Locale")]
  public sealed class Locale : ScriptableObject, ILocale
  {
    // Constans that define defaults for formats

    /// <summary>
    /// The default decimal number format to use when formatting arguments.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings" />
    public const string defaultDecimalNumberFormat = "n";

    /// <summary>
    /// The default percent number format to use when formatting arguments.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings" />
    public const string defaultPercentNumberFormat = "p";

    /// <summary>
    /// The default currency number format to use when formatting arguments.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings" />
    public const string defaultCurrencyNumberFormat = "c";

    /// <summary>
    /// The default short date format to use when formatting arguments.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"/>
    public const string defaultShortDateFormat = "d";

    /// <summary>
    /// The default long date format to use when formatting arguments.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"/>
    public const string defaultLongDateFormat = "D";

    /// <summary>
    /// The default short time format to use when formatting arguments.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"/>
    public const string defaultShortTimeFormat = "t";

    /// <summary>
    /// The default long time format to use when formatting arguments.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"/>
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


    /// <summary>
    /// Return the code of the locale.
    /// </summary>
    public string code => _code;

    /// <summary>
    /// Return the English name of the locale.
    /// </summary>
    public string englishName => _englishName;

    /// <summary>
    /// Return the native name of the locale.
    /// </summary>
    public string nativeName => _nativeName;

    /// <summary>
    /// Return the alternative codes of the locale.
    /// </summary>
    public IReadOnlyDictionary<string, string> altCodes => _altCodes;

    /// <summary>
    /// Return the strings table of the locale.
    /// </summary>
    public ILocalizedStringTable strings => _strings;

    /// <summary>
    /// Return the culture of the locale.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-9.0"/> 
    public CultureInfo culture => CultureInfoExtensions.GetCultureInfoOrInvariant(_code);


    /// <summary>
    /// Return the string representation of the locale.
    /// </summary>
    /// <returns>The string representation of the locale.
    public override string ToString()
    {
      return _englishName;
    }


    #region Message format provider implementation
    /// <summary>
    /// Return the localized string table of the locale.
    /// </summary>
    ILocalizedStringTable IMessageFormatProvider.localizedStringTable => strings;

    /// <summary>
    /// Return a plural rule list for plurals for the locale.
    /// </summary>
    public IPluralizer pluralRules => PluralRuleDatabase.plurals.TryGetRules(this, out var rules) ? rules : null;

    /// <summary>
    /// Return a plural rule list for ordinal plurals for the locale.
    /// </summary>
    public IPluralizer ordinalPluralRules => PluralRuleDatabase.ordinalPlurals.TryGetRules(this, out var rules) ? rules : null;


    /// <summary>
    /// Return the number format for a number format style.
    /// </summary>
    /// <param name="style">The number format style to use.</param>
    /// <returns>The number format corresponding to the specified style</returns>
    public string GetNumberFormat(NumberFormatStyle style)
    {
      return style switch {
        NumberFormatStyle.Decimal => _decimalNumberFormat,
        NumberFormatStyle.Percent => _percentNumberFormat,
        NumberFormatStyle.Currency => _currencyNumberFormat,
        _ => throw new ArgumentException($"Number format style {style} is unsupported"),
      };
    }

    /// <summary>
    /// Return the date format for a date format style.
    /// </summary>
    /// <param name="style">The date format style to use.</param>
    /// <returns>The date format corresponding to the specified style</returns>
    public string GetDateFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => _shortDateFormat,
        DateFormatStyle.Long => _longDateFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    /// <summary>
    /// Return the time format for a date format style.
    /// </summary>
    /// <param name="style">The time format style to use.</param>
    /// <returns>The time format corresponding to the specified style</returns>
    public string GetTimeFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => _shortTimeFormat,
        DateFormatStyle.Long => _longTimeFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    /// <summary>
    /// Return the formatted representation of a number.
    /// </summary>
    /// <param name="number">The number to format.</param>
    /// <param name="style">The number format style to use.</param>
    /// <returns>The formatted representation of the specified number.</returns>
    public string FormatNumber(NumberContext number, NumberFormatStyle style = NumberFormatStyle.Decimal)
    {
      return number.value.ToString(GetNumberFormat(style), culture);
    }

    /// <summary>
    /// Return the formatted representation of a date.
    /// </summary>
    /// <param name="value">The date to format.</param>
    /// <param name="style">The date format style to use.</param>
    /// <returns>The formatted representation of the specified date.</returns>
    public string FormatDate(DateTime value, DateFormatStyle style = DateFormatStyle.Short)
    {
      return value.ToString(GetDateFormat(style), culture);
    }

    /// <summary>
    /// Return the formatted representation of a time.
    /// </summary>
    /// <param name="value">The time to format.</param>
    /// <param name="style">The time format style to use.</param>
    /// <returns>The formatted representation of the specified time.</returns>
    public string FormatTime(DateTime value, DateFormatStyle style = DateFormatStyle.Short)
    {
      return value.ToString(GetTimeFormat(style), culture);
    }
    #endregion

    #region Editor methods
#if UNITY_EDITOR
    /// <summary>
    /// Return all locale assets in the asset database.
    /// </summary>
    /// <param name="searchInFolders">The paths of folders to search in for locale assets.</param>
    /// <returns>An <c>IEnumerable</c> of locale assets found in the specified folders.</returns>
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
