using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Base class that defines a locale selector as a scriptable object
  public abstract class LocaleSelector : ScriptableObject
  {
    // Select a locale according to this selector
    public abstract Locale Select(IReadOnlyList<Locale> locales);

    // Return if a locale could be selected according to this selector and store the locale
    public bool TrySelect(IReadOnlyList<Locale> locales, out Locale locale)
    {
      locale = Select(locales);
      return locale != null;
    }
  }
}
