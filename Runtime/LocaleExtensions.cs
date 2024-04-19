using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  // Class that defines extension methods for locales
  public static class LocaleExtensions
  {
    // Return all strings in all specified locales
    public static IEnumerable<string> GetStrings(this IEnumerable<Locale> locales, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if (locales == null)
        return Enumerable.Empty<string>();

      return locales.SelectMany(locale => locale.strings.keys).Distinct(StringComparer.FromComparison(comparisonType));
    }

    // Return all values for the specified path in all specified locales
    public static IReadOnlyDictionary<Locale, string> GetValues(this IEnumerable<Locale> locales, string path)
    {
      if (locales == null)
        return new Dictionary<Locale, string>();

      return locales
        .Select(locale => new KeyValuePair<Locale, string>(locale, locale.strings.Find(path, null)))
        .Where(e => e.Value != null)
        .ToDictionary();
    }

    // Return if a string reference with the specified path can be found in any of the specified locales
    public static bool ContainsStringInAny(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if (locales == null)
        return false;

      return locales.Any(locale => locale.strings.Contains(path, comparisonType));
    }

    // Return if a string reference with the specified path can be found in all of the specified locales
    public static bool ContainsStringInAll(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if (locales == null)
        return false;

      return locales.All(locale => locale.strings.Contains(path, comparisonType));
    }

    // Return if a string reference with the specified path contains no values
    public static bool ContainsUndefinedString(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      return !locales.ContainsStringInAny(path, comparisonType);
    }

    // Return if a string reference with the specified path contains missing values
    public static bool ContainsMissingString(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      return !locales.ContainsStringInAll(path, comparisonType);
    }
  }
}