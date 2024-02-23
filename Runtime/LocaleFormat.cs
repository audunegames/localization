using System;

namespace Audune.Localization
{
  [Serializable]
  public sealed class LocaleFormat
  {
    // Constans that define defaults for formats
    public const string DefaultDecimalNumberFormat = "n";
    public const string DefaultPercentNumberFormat = "p";
    public const string DefaultCurrencyNumberFormat = "c";
    public const string DefaultShortDateFormat = "d";
    public const string DefaultLongDateFormat = "D";
    public const string DefaultShortTimeFormat = "t";
    public const string DefaultLongTimeFormat = "T";


    // Locale format properties
    public string decimalNumberFormat = DefaultDecimalNumberFormat;
    public string percentNumberFormat = DefaultPercentNumberFormat;
    public string currencyNumberFormat = DefaultCurrencyNumberFormat;
    public string shortDateFormat = DefaultShortDateFormat;
    public string longDateFormat = DefaultLongDateFormat;
    public string shortTimeFormat = DefaultShortTimeFormat;
    public string longTimeFormat = DefaultLongTimeFormat;


    // Return the number format for a number format style
    public string GetNumberFormat(NumberFormatStyle style)
    {
      return style switch {
        NumberFormatStyle.Decimal => decimalNumberFormat,
        NumberFormatStyle.Percent => percentNumberFormat,
        NumberFormatStyle.Currency => currencyNumberFormat,
        _ => throw new ArgumentException($"Number format style {style} is unsupported"),
      };
    }

    // Return the default number format for a number format style
    public static string GetDefaultNumberFormat(NumberFormatStyle style)
    {
      return style switch {
        NumberFormatStyle.Decimal => DefaultDecimalNumberFormat,
        NumberFormatStyle.Percent => DefaultPercentNumberFormat,
        NumberFormatStyle.Currency => DefaultCurrencyNumberFormat,
        _ => throw new ArgumentException($"Number format style {style} is unsupported"),
      };
    }

    // Return the date format for a date format style
    public string GetDateFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => shortDateFormat,
        DateFormatStyle.Long => longDateFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    // Return the default date format for a date format style
    public static string GetDefaultDateFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => DefaultShortDateFormat,
        DateFormatStyle.Long => DefaultLongDateFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    // Return the time format for a date format style
    public string GetTimeFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => shortTimeFormat,
        DateFormatStyle.Long => longTimeFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }

    // Return the default time format for a date format style
    public static string GetDefaultTimeFormat(DateFormatStyle style)
    {
      return style switch {
        DateFormatStyle.Short => DefaultShortTimeFormat,
        DateFormatStyle.Long => DefaultLongTimeFormat,
        _ => throw new ArgumentException($"Date format style {style} is unsupported"),
      };
    }


    // Return the formatted representation of a number
    public string FormatNumber(int value, IFormatProvider formatProvider, NumberFormatStyle style = NumberFormatStyle.Decimal)
    {
      try
      {
        return value.ToString(GetNumberFormat(style), formatProvider);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultNumberFormat(style), formatProvider);
      }
    }
    public string FormatNumber(float value, IFormatProvider formatProvider, NumberFormatStyle style = NumberFormatStyle.Decimal)
    {
      try
      {
        return value.ToString(GetNumberFormat(style), formatProvider);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultNumberFormat(style), formatProvider);
      }
    }

    // Return the formatted representation of a date
    public string FormatDate(DateTime value, IFormatProvider formatProvider, DateFormatStyle style = DateFormatStyle.Short)
    {
      try
      {
        return value.ToString(GetDateFormat(style), formatProvider);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultDateFormat(style), formatProvider);
      }
    }

    // Return the formatted representation of a time
    public string FormatTime(DateTime value, IFormatProvider formatProvider, DateFormatStyle style = DateFormatStyle.Short)
    {
      try
      {
        return value.ToString(GetTimeFormat(style), formatProvider);
      }
      catch (FormatException)
      {
        return value.ToString(GetDefaultTimeFormat(style), formatProvider);
      }
    }
  }
}