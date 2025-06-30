using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a locale selector that returns the specified locale.
  /// </summary>
  [AddComponentMenu("Audune/Localization/Locale Selectors/Specific Locale Selector")]
  public sealed class SpecificLocaleSelector : LocaleSelector
  {
    // Locale selector properties
    [SerializeField, Tooltip("The locale to select")]
    private Locale _locale;


    /// <summary>
    /// Return if a locale could be selected according to this selector and store the selected locale
    /// </summary>
    public override bool TrySelectLocale(IReadOnlyList<ILocale> locales, out ILocale locale)
    {
      locale = _locale;
      return true;
    }
  }
}
