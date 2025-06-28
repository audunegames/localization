using System;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a format provider for a message formatter.
  /// </summary>
  public interface IMessageFormatProvider
  {
    /// <summary>
    /// Return the localized string table of the formatter.
    /// </summary>
    public ILocalizedStringTable localizedStringTable { get; }

    /// <summary>
    /// Return a plural rule list for plurals for the formatter.
    /// </summary>
    public IPluralizer pluralRules { get; }

    /// <summary>
    /// Return a plural rule list for ordinal plurals for the formatter.
    /// </summary>
    public IPluralizer ordinalPluralRules { get; }


    /// <summary>
    /// Return the number format for a number format style.
    /// </summary>
    /// <param name="style">The number format style to use.</param>
    /// <returns>The number format corresponding to the specified style</returns>
    public string GetNumberFormat(NumberFormatStyle style);

    /// <summary>
    /// Return the date format for a date format style.
    /// </summary>
    /// <param name="style">The date format style to use.</param>
    /// <returns>The date format corresponding to the specified style</returns>
    public string GetDateFormat(DateFormatStyle style);

    /// <summary>
    /// Return the time format for a date format style.
    /// </summary>
    /// <param name="style">The time format style to use.</param>
    /// <returns>The time format corresponding to the specified style</returns>
    public string GetTimeFormat(DateFormatStyle style);

    /// <summary>
    /// Return the formatted representation of a number.
    /// </summary>
    /// <param name="number">The number to format.</param>
    /// <param name="style">The number format style to use.</param>
    /// <returns>The formatted representation of the specified number.</returns>
    public string FormatNumber(NumberContext number, NumberFormatStyle style = NumberFormatStyle.Decimal);

    /// <summary>
    /// Return the formatted representation of a date.
    /// </summary>
    /// <param name="value">The date to format.</param>
    /// <param name="style">The date format style to use.</param>
    /// <returns>The formatted representation of the specified date.</returns>
    public string FormatDate(DateTime value, DateFormatStyle style = DateFormatStyle.Short);

    /// <summary>
    /// Return the formatted representation of a time.
    /// </summary>
    /// <param name="value">The time to format.</param>
    /// <param name="style">The time format style to use.</param>
    /// <returns>The formatted representation of the specified time.</returns>
    public string FormatTime(DateTime value, DateFormatStyle style = DateFormatStyle.Short);
  }
}