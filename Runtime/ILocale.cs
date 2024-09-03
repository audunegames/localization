using System.Collections.Generic;
using System.Globalization;

namespace Audune.Localization
{
  // Interface that defines a locale
  public interface ILocale : IMessageFormatProvider
  {
    // Return the code of the locale
    public string code { get; }

    // Return the English name of the locale
    public string englishName { get; }

    // Return the native name of the locale
    public string nativeName { get; }

    // Return the alternative codes of the locale
    public IReadOnlyDictionary<string, string> altCodes { get; }

    // Return the strings table of the locale
    public ILocalizedStringTable strings { get; }

    // Return the culture of the locale
    public CultureInfo culture => CultureInfoExtensions.GetCultureInfoOrInvariant(code);
  }
}
