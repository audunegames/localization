using System;
using System.Collections.Generic;
using System.Linq;

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

      return enumerable.SelectMany(InterleaveFunction);
    }
  }
}