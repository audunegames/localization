using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Base class that defines a locale loader as a scriptable object
  public abstract class LocaleLoader : ScriptableObject
  {
    // Load locales according to this loader
    public abstract IEnumerable<Locale> Load();
  }
}
