using System;
using System.Collections.Generic;

namespace Audune.Localization
{
  // Class that defines extension methods for localized reference lists
  public static class LocalizedReferenceListExtensions
  {
    // Join multiple localized references
    public static ILocalizedReference<TValue> Join<TValue>(this IEnumerable<ILocalizedReference<TValue>> references, Func<TValue, TValue, TValue> aggregator)
    {
      return new LocalizedReferenceList<TValue>(references, aggregator);
    }

    // Join multiple localized string references
    public static ILocalizedReference<string> Join(this IEnumerable<ILocalizedReference<string>> references, string separator = "")
    {
      return Join(references, (a, b) => a + separator + b);
    }
  }
}