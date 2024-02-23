using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a locale selector that returns the specified locale
  [AddComponentMenu("Audune/Localization/Locale Selectors/Specific Locale Selector")]
  public sealed class SpecificLocaleSelector : LocaleSelector
  {
    // Locale selector properties
    [SerializeField, Tooltip("The locale to select")]
    private Locale _locale;


    // Return if a locale could be selected according to this selector and store the selected locale
    public override bool TrySelectLocale(IReadOnlyList<Locale> locales, out Locale locale)
    {
      locale = _locale;
      return true;
    }
  }
}
