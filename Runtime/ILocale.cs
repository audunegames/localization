using System.Collections.Generic;
using System.Globalization;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a locale.
  /// </summary>
  public interface ILocale : IMessageFormatProvider
  {
    /// <summary>
    /// Return the code of the locale.
    /// </summary>
    public string code { get; }

    /// <summary>
    /// Return the English name of the locale.
    /// </summary>
    public string englishName { get; }

    /// <summary>
    /// Return the native name of the locale.
    /// </summary>
    public string nativeName { get; }

    /// <summary>
    /// Return the alternative codes of the locale.
    /// </summary>
    public IReadOnlyDictionary<string, string> altCodes { get; }

    /// <summary>
    /// Return the strings table of the locale.
    /// </summary>
    public ILocalizedStringTable strings { get; }

    /// <summary>
    /// Return the culture of the locale.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-9.0"/> 
    public CultureInfo culture { get; }
  }
}
