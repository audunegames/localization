using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Base class that defines a locale loader
  [RequireComponent(typeof(LocalizationSystem))]
  public abstract class LocaleLoader : MonoBehaviour
  {
    // Locale selector properties
    [SerializeField, Tooltip("The priority of the locale loader")]
    private int _priority;
    [SerializeField, Tooltip("The execution mode of the locale loader")]
    private ExecutionMode _executionMode = ExecutionMode.Always;


    // Return the priority of the loader
    public int priority => _priority;

    // Return the execution mode of the loader
    public ExecutionMode executionMode => _executionMode;


    // Load locales according to this loader
    public abstract IEnumerable<ILocale> LoadLocales();
  }
}
