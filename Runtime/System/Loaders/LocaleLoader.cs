using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Base class that defines a locale loader.
  /// </summary>
  [RequireComponent(typeof(LocalizationSystem))]
  public abstract class LocaleLoader : MonoBehaviour
  {
    // Locale selector properties
    [SerializeField, Tooltip("The priority of the locale loader")]
    private int _priority;
    [SerializeField, Tooltip("The execution mode of the locale loader")]
    private ExecutionMode _executionMode = ExecutionMode.Always;


    /// <summary>
    /// Return the priority of the loader.
    /// </summary>
    public int priority => _priority;

    /// <summary>
    /// Return the execution mode of the loader.
    /// </summary>
    public ExecutionMode executionMode => _executionMode;


    /// <summary>
    /// Load locales according to this loader.
    /// </summary>
    /// <returns>An enumerable of loaded locales.</returns>
    public abstract IEnumerable<ILocale> LoadLocales();
  }
}
