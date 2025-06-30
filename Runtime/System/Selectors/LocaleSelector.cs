using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Base class that defines a locale selector.
  /// </summary>
  [RequireComponent(typeof(LocalizationSystem))]
  public abstract class LocaleSelector : MonoBehaviour
  {
    // Locale selector properties
    [SerializeField, Tooltip("The priority of the locale selector")]
    private int _priority;
    [SerializeField, Tooltip("The execution mode of the locale selector")]
    private ExecutionMode _executionMode = ExecutionMode.Always;


    /// <summary>
    /// Return the priority of the selector.
    /// </summary>
    public int priority => _priority;

    /// <summary>
    /// Return the execution mode of the selector.
    /// </summary>
    public ExecutionMode executionMode => _executionMode;


    /// <summary>
    /// Return if a locale could be selected according to this selector and store the selected locale.
    /// </summary>
    /// <param name="locales">The list of locales to select a locale from.</param>
    /// <param name="locale">The locale in which the selected locale will be stored if a locale could be selected.</param>
    /// <returns>Whether a locale could be selected.</returns>
    public abstract bool TrySelectLocale(IReadOnlyList<ILocale> locales, out ILocale locale);
  }
}
