using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Audune.Localization
{
  // Class that defines extension methods for enumerables
  internal static class EnumerableExtensions
  {
    // Interlace an enumerable of values with a separator
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

    // Concatenate an enumerable of items mapped to a string using the specified selector
    public static string Concatenate<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, string> stringSelector = null)
    {
      if (enumerable == null)
        throw new ArgumentNullException(nameof(enumerable));

      stringSelector ??= v => v.ToString();

      return enumerable.Aggregate(new StringBuilder(), (b, v) => b.Append(stringSelector(v)), b => b.ToString());
    }

    // Concatenate an enumerable of strings
    public static string Concatenate(this IEnumerable<string> enumerable)
    {
      return enumerable.Concatenate(s => s);
    }
  }
}