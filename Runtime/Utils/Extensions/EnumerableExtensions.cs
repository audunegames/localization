using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines extension methods for enumerables.
  /// </summary>
  internal static class EnumerableExtensions
  {
    /// <summary>
    /// Interleavee an enumerable of values with a separator.
    /// </summary>
    /// <param name="enumerable">The enumerable to interleave.</param>
    /// <param name="separator">The separator to interleave.</param>
    /// <returns>The interleaved enumerable.</returns>
    public static IEnumerable<TValue> Interleave<TValue>(this IEnumerable<TValue> enumerable, TValue separator)
    {
      if (enumerable == null)
        throw new ArgumentNullException(nameof(enumerable));

      IEnumerable<TValue> InterleaveFunction(TValue value, int index) 
      {
        if (index > 0)
          yield return separator;
        yield return value;
      };

      return separator != null ? enumerable.SelectMany(InterleaveFunction) : enumerable;
    }

    /// <summary>
    /// Concatenate an enumerable of items mapped to a string using the specified selector.
    /// </summary>
    /// <param name="enumerable">The enumerable to concatenate.</param>
    /// <param name="stringSelector">The selector for selecting a string for each item in the enumerable.</param>
    /// <returns>The concatenated string.</returns>
    public static string Concatenate<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, string> stringSelector = null)
    {
      if (enumerable == null)
        throw new ArgumentNullException(nameof(enumerable));

      stringSelector ??= v => v.ToString();

      return enumerable.Aggregate(new StringBuilder(), (b, v) => b.Append(stringSelector(v)), b => b.ToString());
    }

    /// <summary>
    /// Concatenate an enumerable of strings.
    /// </summary>
    /// <param name="enumerable">The enumerable to concatenate.</param>
    /// <returns>The concatenated string.</returns>
    public static string Concatenate(this IEnumerable<string> enumerable)
    {
      return enumerable.Concatenate(s => s);
    }
  }
}