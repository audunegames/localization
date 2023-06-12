using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization.Selectors
{
  // Class that defines a locale selector that returns the specified locale
  public sealed class SpecificLocaleSelector : LocaleSelector
  {
    // Locale selector settings
    [SerializeField, Tooltip("The locale to select")]
    private Locale _locale;


    // Return the locale according to this selector
    public override Locale Select(IReadOnlyList<Locale> locales)
    {
      // Return the specified locale
      return _locale;
    }
  }
}
