using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a locale loader that loads the specified locales
  [AddComponentMenu("Audune/Localization/Locale Loaders/Persistent Locale Loader")]
  public sealed class PersistentLocaleLoader : LocaleLoader
  {
    // Locale loader properties
    [SerializeField, Tooltip("The list of locales to load")]
    private List<Locale> _locales;


    // Load locales according to this loader
    public override IEnumerable<ILocale> LoadLocales()
    {
      // Return the locales in the list
      return _locales.Where(l => l != null);
    }
  }
}
