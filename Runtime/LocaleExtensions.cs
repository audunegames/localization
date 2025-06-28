using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines extension methods for locales.
  /// </summary>
  public static class LocaleExtensions
  {
    /// <summary>
    /// Return all strings in all specified locales.
    /// </summary>
    /// <param name="locales">The locales to get the strings from.</param>
    /// <param name="comparisonType">The string comparison type to compare equal strings with.</param>
    /// <returns>An <c>IEnumerable</c> of all strings found in the specified locales.</returns>
    public static IEnumerable<string> GetStrings(this IEnumerable<Locale> locales, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if (locales == null)
        return Enumerable.Empty<string>();

      return locales.SelectMany(locale => locale.strings.keys).Distinct(StringComparer.FromComparison(comparisonType));
    }

    /// <summary>
    /// Return all values for the specified path in all specified locales.
    /// </summary>
    /// <param name="locales">The locales to get the values from.</param>
    /// <param name="path">The path of the string reference.</param>
    /// <returns>A <c>IReadOnlyDictionary</c> of the specified locales mapped to the respective values of the specified path.</returns>
    public static IReadOnlyDictionary<Locale, string> GetValues(this IEnumerable<Locale> locales, string path)
    {
      if (locales == null)
        return new Dictionary<Locale, string>();

      return locales
        .Select(locale => new KeyValuePair<Locale, string>(locale, locale.strings.Find(path, null)))
        .Where(e => e.Value != null)
        .ToDictionary();
    }

    /// <summary>
    /// Return if a string reference with the specified path can be found in any of the specified locales.
    /// </summary>
    /// <param name="locales">The locales to find the string reference in.</param>
    /// <param name="path">The path of the string reference.</param>
    /// <param name="comparisonType">The string comparison type to compare equal strings with.</param>
    /// <returns>If the specified string reference could be found in any of the specified locales.</returns>
    public static bool ContainsStringInAny(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if (locales == null)
        return false;

      return locales.Any(locale => locale.strings.Contains(path, comparisonType));
    }

    /// <summary>
    /// Return if a string reference with the specified path can be found in all of the specified locales.
    /// </summary>
    /// <param name="locales">The locales to find the string reference in.</param>
    /// <param name="path">The path of the string reference.</param>
    /// <param name="comparisonType">The string comparison type to compare equal strings with.</param>
    /// <returns>If the specified string reference could be found in all specified locales.</returns>
    public static bool ContainsStringInAll(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if (locales == null)
        return false;

      return locales.All(locale => locale.strings.Contains(path, comparisonType));
    }

    /// <summary>
    /// Return if a string reference with the specified path contains no values.
    /// </summary>
    /// <param name="locales">The locales to find the string reference in.</param>
    /// <param name="path">The path of the string reference.</param>
    /// <param name="comparisonType">The string comparison type to compare equal strings with.</param>
    /// <returns>If all specified locales have no value for the specified string reference, i.e. the negation of <c>ContainsStringInAny</c>.</returns>
    public static bool ContainsUndefinedString(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      return !locales.ContainsStringInAny(path, comparisonType);
    }

    /// <summary>
    /// Return if a string reference with the specified path contains missing values.
    /// </summary>
    /// <param name="locales">The locales to find the string reference in.</param>
    /// <param name="path">The path of the string reference.</param>
    /// <param name="comparisonType">The string comparison type to compare equal strings with.</param>
    /// <returns>If any of the specified locales has no value for the specified string reference, i.e. the negation of <c>ContainsStringInAll</c>.</returns>
    public static bool ContainsMissingString(this IEnumerable<Locale> locales, string path, StringComparison comparisonType = StringComparison.Ordinal)
    {
      return !locales.ContainsStringInAll(path, comparisonType);
    }
  }
}