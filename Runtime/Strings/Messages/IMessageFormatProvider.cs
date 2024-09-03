using System;

namespace Audune.Localization
{
  // Interface that defines a format provider for a message formatter
  public interface IMessageFormatProvider
  {
    // Return the localized string table of the formatter
    public ILocalizedStringTable localizedStringTable { get; }

    // Return the plural rules of the formatter
    public IPluralizer pluralRules { get; }

    // Return the ordinal plural rules of the formatter
    public IPluralizer ordinalPluralRules { get; }


    // Return the number format for a number format style
    public string GetNumberFormat(NumberFormatStyle style);

    // Return the date format for a date format style
    public string GetDateFormat(DateFormatStyle style);

    // Return the time format for a date format style
    public string GetTimeFormat(DateFormatStyle style);

    // Return the formatted representation of a number
    public string FormatNumber(NumberContext number, NumberFormatStyle style = NumberFormatStyle.Decimal);

    // Return the formatted representation of a time
    public string FormatTime(DateTime value, DateFormatStyle style = DateFormatStyle.Short);

    // Return the formatted representation of a date
    public string FormatDate(DateTime value, DateFormatStyle style = DateFormatStyle.Short);
  }
}