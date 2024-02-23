using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Base class that defines a locale selector
  [RequireComponent(typeof(LocalizationSystem))]
  public abstract class LocaleSelector : MonoBehaviour
  {
    // Locale selector properties
    [SerializeField, Tooltip("The priority of the locale selector")]
    private int _priority;
    [SerializeField, Tooltip("The execution mode of the locale selector")]
    private ExecutionMode _executionMode = ExecutionMode.Always;


    // Return the priority of the selector
    public int priority => _priority;

    // Return the execution mode of the selector
    public ExecutionMode executionMode => _executionMode;


    // Return if a locale could be selected according to this selector and store the selected locale
    public abstract bool TrySelectLocale(IReadOnlyList<Locale> locales, out Locale locale);
  }
}
