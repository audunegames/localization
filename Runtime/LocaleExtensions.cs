using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  // Class that defines extension methods for locales
  public static class LocaleExtensions
  {
    // Return if a string reference with the specified path can be found in any of the specified locales
    public static bool ContainsStringInAny(this IEnumerable<Locale> locales, string path)
    {
      if (locales == null)
        return false;

      return locales.Any(locale => locale.strings.Contains(path));
    }

    // Return if a string reference with the specified path can be found in all of the specified locales
    public static bool ContainsStringInAll(this IEnumerable<Locale> locales, string path)
    {
      if (locales == null)
        return false;

      return locales.All(locale => locale.strings.Contains(path));
    }

    // Return if a string reference with the specified path contains no values
    public static bool ContainsUndefinedString(this IEnumerable<Locale> locales, string path)
    {
      return !locales.ContainsStringInAny(path);
    }

    // Return if a string reference with the specified path contains missing values
    public static bool ContainsMissingString(this IEnumerable<Locale> locales, string path)
    {
      return !locales.ContainsStringInAll(path);
    }
  }
}