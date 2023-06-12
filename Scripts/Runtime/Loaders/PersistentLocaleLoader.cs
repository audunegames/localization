using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization.Loaders
{
  // Class that defines a locale loader that loads the specified locales
  public sealed class PersistentLocaleLoader : LocaleLoader
  {
    // Locale loader settings
    [SerializeField, Tooltip("The list of locales to load")]
    private List<Locale> _locales;


    // Load locales according to this loader
    public override IEnumerable<Locale> Load()
    {
      // Return the locales in the list
      return _locales;
    }
  }
}
