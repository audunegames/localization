namespace Audune.Localization
{
  /// <summary>
  /// Enum that defines the style of a message component that defines a number format argument.
  /// </summary>
  public enum NumberFormatStyle
  {
    /// <summary>
    /// The decimal number format style, e.g. "1.23".
    /// </summary>
    Decimal,

    /// <summary>
    /// The percent number format style, e.g. "123%".
    /// </summary>
    Percent,

    /// <summary>
    /// The currency number format style, e.g. "$1.23".
    /// </summary>
    Currency,
  }
}